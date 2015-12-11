#include "matrix_comput.h"
#include "type_utils.h"
#include <vector>
#include <complex>

using namespace std;

struct SPolesZerosProcOpts
{
	bool isPolesReflection;		//Flag specifying whether all the poles placed outside unit circle will be reflected into it
	bool isZerosReflection;		//Flag specifying whether all the zeros placed outside unit circle will be reflected into it
	double MaxPoleRad;			//Maximum value of pole radius (not referenced if isPolesReflection == false)
	double MaxZeroRad;			//Maximum value of zero radius (not referenced if isZerosReflection == false)
};
struct SNewtonStopConds
{
	double Y_tol;		//Tolerance for objective function value
	double gradX_tol;	//Tolerance for gradient Euclidean norm
	double dX_tol;		//Tolerance for step vector Euclidean norm
	double dY_tol;		//Tolerance for objective function decrement
	
	int IterLimit;		//Acceptable number of iterations (0 equivalent to unlimited number of iterations)
	int TimeLimit;		//Acceptable execution time in milliseconds (0 equivalent to unlimited time)
};
struct SLineSearchStopConds
{
	double dX_st;		//Initial value for simplex length,
	double dX_tol;		//Tolerance for simplex length,
	int IterLimit;		//Acceptable number of iterations (0 equivalent to unlimited number of iterations)
};

using std::vector;
using std::numeric_limits;
using std::complex;

#define pi 3.1415926535897931
#define jot complex<double>(0,1)
#define inf numeric_limits<double>::infinity()
#define nan numeric_limits<double>::quiet_NaN()

#define uint unsigned int

double norm
(
	const vector<double> &x
)
{
	size_t len = x.size();

	double res = 0;
	for (uint i = 0; i < len; i++)
		res += x[i] * x[i];

	return sqrt(res);
}


vector<double> GetInitCoefs_aux
(
	unsigned int ord,
	double rad,
	double k,
	int s
)
{
	if (rad == inf)
		rad = 1;
	
	rad *= k;

	vector<double> c(ord+1);
	c[0] = 1;
	if (ord != 0)
		c[ord] = s * pow(rad, (int)ord);

	return c;
};

void GetInitCoefs
//	Choose initial values of polynomials coefficients
//	(poles and zeros are placed on circles)
(
	unsigned int NumOrd,
	unsigned int DenOrd,
	double MaxPoleRad,
	double MaxZeroRad,
	
	vector<double> &b,
	vector<double> &a
)
{
	if ( _isnan(b[0]) )
		b = GetInitCoefs_aux(NumOrd, MaxZeroRad, 0.6, -1);
	if ( _isnan(a[0]) )
		a = GetInitCoefs_aux(DenOrd, MaxPoleRad, 0.4,  1);
}	


vector<bool> GetEdgeFreqFlagArray
//	Prepare array of flags specifying whether each frequency corresponds to band edge
(
	size_t FreqNum,
	double *f,
	double *edges
)
{
	vector<bool> isEdgeFreq(FreqNum);
	isEdgeFreq[0] = isEdgeFreq[FreqNum-1] = true;

	int ei = 1;
	for (uint i = 1; i < FreqNum-1; i++)
	{
		if ( f[i] == edges[ei] )
		{
			isEdgeFreq[i] = true;
			ei++;
		}
		else
			isEdgeFreq[i] = false;
	}

	return isEdgeFreq;
}

vector<double> GetIntWeiArray
//	Prepare array of weights corresponding to frequency intervals.
//	Non-optimizable intervals are marked with zero weight.
(
	size_t FreqNum,
	double *f,
	size_t EdgesNum,
	double *edges,
	double *w
)
{
	//Prepare array of flags specifying whether each frequency corresponds to band edge
		vector<bool> isEdgeFreq = GetEdgeFreqFlagArray(FreqNum, f, edges);

	//Number of frequency intervals
		size_t IntNum = FreqNum - 1;

	vector<double> IntWei(IntNum);
	IntWei[0] = w[0];

	int wi = 0;
	bool flag = true;
	for (uint i = 1; i < IntNum; i++)
	{
		if ( isEdgeFreq[i] )
			flag = !flag;
		if ( flag == true )
			wi++;
		IntWei[i] = flag * w[wi];
	}

	return IntWei;
}

void GetGrids
//	Prepare global arrays of z, M and w according to specified PtsPerIntNum value.
//	Non-optimizable intervals are excluded from the grids.
(
	size_t FreqNum,
	double *f,
	size_t EdgesNum,
	double *edges,
	double *M,
	size_t OptIntNum,
	double *w,
	size_t PtsPerIntNum,

	vector<complex<double>> &z_grid,
	vector<double> &M_grid,
	vector<double> &w_grid 
)
{
	//Prepare array of weights corresponding to frequency intervals.
	//Non-optimizable intervals are marked with zero weight.
		vector<double> IntWei = GetIntWeiArray(FreqNum, f, EdgesNum, edges, w);

	//Initialize global variable GridLen,
	//allocate memory for global arrays z_grid, M_grid, w_grid
		size_t GridLen = OptIntNum * PtsPerIntNum;
		z_grid.resize(GridLen);
		M_grid.resize(GridLen);
		w_grid.resize(GridLen); 

	size_t FullIntNum = FreqNum - 1;
	double df, f_, dM, M_;

	int j0 = 0;
	for (uint IntInd = 0; IntInd < FullIntNum; IntInd++)
	//Loop over frequency intervals
	{
		if ( IntWei[IntInd] == 0 )
		//This is nonoptimizable interval - skip it
			continue;

		//Specify step of frequency grid and the first value of f on the interval
			df = ( f[IntInd+1] - f[IntInd] ) / PtsPerIntNum;
			f_ = f[IntInd] + df/2;
			
		//Specify step of transfer function modulus and the first value of M on the interval
			dM = ( M[IntInd+1] - M[IntInd] ) / PtsPerIntNum;
			M_ = M[IntInd] + dM/2;

		for (uint i = 0; i < PtsPerIntNum; i++)
		//Loop over points of the interval
		{
			z_grid[j0+i] = exp(jot*pi*f_);
			f_ += df;
			M_grid[j0+i] = M_;
			M_ += dM;
			w_grid[j0+i] = IntWei[IntInd];
		}
		j0 += PtsPerIntNum;
	}
}

#define SWAP(g,h) {y=(g);(g)=(h);(h)=y;}
void elmhes(double *a, int n)
{
    int m, j, i;
    double y, x;
    for(m = 1; m < n-1; m++)
    {
        x = 0.0;
        i = m;
        for(j = m; j < n; j++)
        {
            if (fabs(a[j*n + (m-1)]) > fabs(x))
            {
                x = a[j*n + (m-1)];
                i = j;
            }
        }
        if(i != m) 
        { 
            for(j = m-1; j < n; j++) SWAP(a[i*n + j], a[m*n + j])
            for(j = 0; j < n; j++) SWAP(a[j*n + i], a[j*n + m])
        }
        if(x)
        {
            for(i = m+1; i < n; i++)
            {
                if((y=a[i*n + (m-1)]) != 0.0)
                {
                    y /= x;
                    a[i*n + (m-1)] = y;
                    //a[i*n + (m-1)] = 0;
                    for(j = m; j < n; j++)
                        a[i*n + j] -= y*a[m*n + j];
                    for(j = 0; j < n; j++)
                        a[j*n + m] += y*a[j*n + i];
                }
            }
        }
    }
}
#define IMAX(a,b) (a) > (b) ? (a) : (b) 
#define IMIN(a,b) (a) > (b) ? (b) : (a)
#define SIGN(a,b) ((b) >= 0.0 ? fabs(a) : -fabs(a))

int hqr(double *a, int n, double *wr, double *wi)
{
    int nn, m, l, k, j, its, i, mmin;
    double z,y,x,w,v,u,t,s,r,q,p,anorm;

    anorm=0.0;
    for(i = 0; i < n; i++)
        for(j = IMAX(i-1,0); j < n; j++)
            anorm += fabs(a[i*n+j]);
    nn = n-1;
    t  = 0.0;
    while (nn >= 0)
    {
        its = 0;
        do
        {
            for (l = nn; l >= 1; l--)
            {
                s = fabs(a[(l-1)*n + l-1])+fabs(a[l*n+l]);
                if(s == 0.0) s = anorm;
                if((fabs(a[l*n + l-1]) + s) == s)
                {
                    a[l*n + l-1]=0.0;
                    break;
                }
            }
            x=a[nn*n+nn];
            if (l == nn)
            { //One root found
                wr[nn]=x+t;
                wi[nn]=0.0;
                nn--;
            }
            else
            {
                y=a[(nn-1)*n + nn-1];
                w=a[nn*n+nn-1]*a[(nn-1)*n+nn];
                if (l == (nn-1))
                { // Two roots found
                    p=0.5*(y-x);
                    q=p*p+w;
                    z=sqrt(fabs(q));
                    x += t;
                    if (q >= 0.0)
                    { // ... a real pair
                        z = p + SIGN(z,p);
                        wr[nn]=x+z;
                        wr[nn-1]=wr[nn];
                        if (z) wr[nn]=x-w/z;
                        wi[nn]=0.0;
                        wi[nn-1]=0.0;
                    }
                    else
                    { //... a complex pair
                        wr[nn-1]=wr[nn]=x+p;
                        wi[nn-1]= -(wi[nn]=z);
                    }
                    nn -= 2;
                }
                else
                { // No roots found. Continue iteration.
                    if (its == 30) return -1; 
                    if (its == 10 || its == 20)
                    {
                        t += x;
                        for (i = 0; i < n; i++) a[i*n +i] -= x;
                        s = fabs(a[nn*n+nn-1])+fabs(a[(nn-1)*n+nn-2]);
                        y = x=0.75*s;
                        w = -0.4375*s*s;
                    }
                    ++its;
                    for (m=(nn-2); m>=l; m--)
                    {
                        z=a[m*n+m];
                        r=x-z;
                        s=y-z;
                        p=(r*s-w)/a[(m+1)*n+m]+a[m*n+m+1];
                        q=a[(m+1)*n+m+1]-z-r-s;
                        r=a[(m+2)*n+m+1];
                        s=fabs(p)+fabs(q)+fabs(r);
                        p /= s;
                        q /= s;
                        r /= s;
                        if (m == l) break;
                        u=fabs(a[m*n+m-1])*(fabs(q)+fabs(r));
                        v=fabs(p)*(fabs(a[(m-1)*n+m-1])+fabs(z)+fabs(a[(m+1)*n+m+1]));
                        if ((u+v) == v) break;
                    }
                    for (i=m+2;i<=nn;i++)
                    {
                        a[i*n+i-2]=0.0;
                        if (i != (m+2)) a[i*n+i-3]=0.0;
                    }
                    for (k=m;k<=nn-1;k++)
                    {
                        if (k != m)
                        {
                            p=a[k*n+k-1];
                            q=a[(k+1)*n+k-1];
                            r=0.0;
                            if (k != (nn-1)) r=a[(k+2)*n+k-1];
                            if ((x=fabs(p)+fabs(q)+fabs(r)) != 0.0)
                            {
                                p /= x;
                                q /= x;
                                r /= x;
                            }
                        }
                    
                    if ((s=SIGN(sqrt(p*p+q*q+r*r),p)) != 0.0)
                    {
                        if (k == m)
                        {
                            if (l != m) a[k*n+k-1] = -a[k*n+k-1];
                        }
                        else
                        {
                            a[k*n+k-1] = -s*x;
                        }
                        p += s;
                        x=p/s;
                        y=q/s;
                        z=r/s;
                        q /= p;
                        r /= p;
                        for (j=k;j<=nn;j++) 
                        {// Row modification
                            p=a[k*n+j]+q*a[(k+1)*n+j];
                            if (k != (nn-1))
                            {
                                p += r*a[(k+2)*n+j];
                                a[(k+2)*n+j] -= p*z;
                            }
                            a[(k+1)*n+j] -= p*y;
                            a[k*n+j] -= p*x;
                        }
                        mmin = nn<k+3 ? nn : k+3;
                        for (i=l;i<=mmin;i++)
                        {
                            p=x*a[i*n+k]+y*a[i*n+k+1];
                            if (k != (nn-1)) {
                            p += z*a[i*n+k+2];
                            a[i*n+k+2] -= p*r;
                            }
                            a[i*n+k+1] -= p*q;
                            a[i*n+k] -= p;
                        }
                    }
                }
            }
        }

    }while(l < nn-1);
}

    return 0;
}

vector<complex<double>> roots(	const vector<double> &c)
{
	uint nc = c.size();
	uint nz = nc - 1;
	vector<complex<double>> r(nz);

	if (nz == 0)
		return r;

	//Count trailing zero coefficients
	//( c[0] always != 0 )
		uint ntz = 0;
		for (uint i = nc-1; i >= 1; i--)
			if (c[i] == 0)
				ntz++;
			else
				break;

	//Strip trailing zeros, but remember them as roots at zero.
		uint n = nz - ntz;

	//Allocate memory for companion matrix
	//The matrix is initially filled up with zeros
		vector<double> a(n*n);

	//Compose companion matrix
		for (uint i = 0; i < n-1; i++)
		{
			a[i] = -c[i+1] / c[0];
			a[(i+1)*n+i] = 1;
		}
		a[n-1] = -c[n] / c[0];

	//Prepare to launching MKL function 'dgebal' (see p. 847 of MKL manual)
    vector<double> scale(n);

	//Balance a general matrix to improve the accuracy
	//of computed eigenvalues
		//dgebal( &job, &MKL_n, &a[0], &lda,   &ilo, &ihi, &scale[0], &info );
		//dgebal( char *job, MKL_INT *n, double *a, MKL_INT *lda, MKL_INT *ilo, MKL_INT *ihi, double *scale, MKL_INT *info );

	//Reduce a general matrix to upper Hessenberg form
	//(or query for workspace array size)
    elmhes(&a[0], n);
		//dgehrd( &MKL_n, &ilo, &ihi, &a[0], &lda, &tau[0], &work[0], &lwork, &info );
		//dgehrd( MKL_INT *n, MKL_INT *ilo, MKL_INT *ihi, double *a, MKL_INT *lda, double *tau, double *work, MKL_INT *lwork, MKL_INT *info );

		vector<double> wr(n);
		vector<double> wi(n);
	//Compute all eigenvalues of a matrix reduced to Hessenberg form
    int reth = hqr(&a[0], n, &wr[0], &wi[0]);
        //dhseqr(&job, &compz, &MKL_n, &ilo, &ihi, &a[0], &ldh, &wr[0], &wi[0], z, &ldz, &work[0], &lwork, &info);

	//Copy the roots in output vector
		for (uint i = 0; i < n; i++)
			r[i] = complex<double>(wr[i], wi[i]);

	return r;
}

vector<double> Roots2SecPolyCoefs
//	Convert the roots to coefficients of SOS-polynomial
(
	const vector<complex<double>> &r,
	bool isDeg
)
{
	size_t ord = r.size();

	uint J = (ord+1) >> 1;
	vector<double> C(2*J);

	if (J == 0)
		return C;

	//Make a copy of const roots vector because roots sequence may be changed
		vector<complex<double>> r_ = r;
		
	//Move all real roots to the end of array
	//(Each section must have real coefficiets and thus section roots
	//must be complex-conjugated or both real)
		uint end = ord;
		for (uint i = 0; i < end-1; )
		{
			if ( r_[i].imag() == 0 )
			//Move this root to the end of array
			{
				r_.push_back( r_[i] );
				r_.erase( r_.begin() + i );
				end--;
			}
			else
				i++;
		}

	if (isDeg)
	//The last section is degenerated (has root at zero)
		r_.push_back(0);

	for (uint i = 0; i < J; i++)
	//Loop over the sections
	//(Function real() is used only for conversion from complex<double> to double)
	{
		C[i]   = -real( r_[2*i] + r_[2*i+1] );
		C[J+i] =  real( r_[2*i] * r_[2*i+1] );
	}
	
	//(Degenerated coefficient is marked up with 0)

	return C;
}

double ExtPolyCoefs2SecPolyCoefs_aux(const vector<double> &c,
                                     vector<double> &C,
                                     bool isDeg)
{
	double h = c[0];
 
	if (c.size() == 1)
		return h;
	
	//Compute roots of polynomial
	//(complex conjugate roots are placed in adjacent cells of the array)
		vector<complex<double>> r = roots(c);
		
	//Convert the roots to coefficients of SOS-polynomial
		C = Roots2SecPolyCoefs(r, isDeg);

	return h;
}

void ExtPolyCoefs2SecPolyCoefs
(
 	const vector<double> &b,
	const vector<double> &a,

	double &H0,
	vector<double> &B,
	vector<double> &A
)
{
	bool isNumDeg = !(b.size() & 1);
	bool isDenDeg = !(a.size() & 1);

	double hNum = ExtPolyCoefs2SecPolyCoefs_aux(b,   B, isNumDeg);
	double hDen = ExtPolyCoefs2SecPolyCoefs_aux(a,   A, isDenDeg);
	H0 = hNum / hDen;
}

complex<double> dR_dx
(
	uint i,
	uint j,
	complex<double> z,

	const vector<complex<double>> &SecVal
)
{
	size_t J = SecVal.size();

	complex<double> v = 1;
	for (uint SecInd = 0; SecInd < J; SecInd++)
	{
		if (SecInd == i) 
			continue;
		v = v * SecVal[SecInd];
	}

	v = v / pow( z, j );
	
	return v;
}

//	1st derivatives of absR

double dabsR_dx
(
	uint i,
	uint j,
	complex<double> z,

	complex<double> R,
	const vector<complex<double>> &SecVal,
	double absR
)
{
	return real( conj(R) * dR_dx(i, j, z,   SecVal) ) / absR;
}

//	1st derivatives of D

inline double dD_dH
(
 	bool isRoughOF,
	double signH0,
	double absP,
	double absQ
)
{
	return isRoughOF ?
		signH0 * absP :
		signH0 * absP / absQ;
}
	
inline double dD_db
(
	uint i, 
	uint j,
	complex<double> z,

	bool isRoughOF,
	complex<double> P,
	const vector<complex<double>> &NumSecVal,
	double absH0,
	double absP,
	double absQ
)
{
	return isRoughOF ?
		absH0 * dabsR_dx(i, j, z,   P, NumSecVal, absP) :
		absH0/absQ * dabsR_dx(i, j, z,   P, NumSecVal, absP);
}

inline double dD_da
(
	uint i, 
	uint j,
	complex<double> z,

	bool isRoughOF,
	complex<double> Q,
	const vector<complex<double>> &DenSecVal,
	double absH0,
	double absP,
	double absQ,
	double M
)
{
	return isRoughOF ?
		-M * dabsR_dx(i, j, z,   Q, DenSecVal, absQ) :
		-absH0*absP / pow( absQ, 2 ) * dabsR_dx(i, j, z,   Q, DenSecVal, absQ);
}

//	1st derivatives of FI

inline double dFI_dx
(
	char bId,
	uint i,
	uint j,
	complex<double> z,

	int p,
	bool isRoughOF,
	double M,
	double D,
	complex<double> P,
	complex<double> Q,
	const vector<complex<double>> &NumSecVal,
	const vector<complex<double>> &DenSecVal,
	double absH0,
	double signH0,
	double absP,
	double absQ
)
{
	double dD_dx;

	switch (bId)
	{
		case 'H':
			dD_dx = dD_dH(isRoughOF, signH0, absP, absQ);
			break;
		case 'b':
			dD_dx = dD_db(i, j, z,   isRoughOF, P, NumSecVal, absH0, absP, absQ);
			break;
		default:	//case 'a':
			dD_dx = dD_da(i, j, z,   isRoughOF, Q, DenSecVal, absH0, absP, absQ, M);
	}

	return p * pow(D, p-1) * dD_dx;
}

int sign(double x)
{
	int res;

	if (x > 0)
		res = 1;
	else if (x < 0)
		res = -1;
	else
		res = 0;

	return res;
}

complex<double> PolyVal(
	const vector<double> &C,
	complex<double> z,

	vector<complex<double>> &SecVal
)
{		
	size_t J = C.size() >> 1;

	complex<double> R = 1;

	if (J != 0)
	{
		complex<double> Z1, Z2;
		Z1 = 1.0 / z;
		Z2 = Z1 * Z1;
		
		for (uint i = 0; i < J; i++)
		//Loop over the sections.
		//(Degenerated coefficient is marked up with 0)
			R *= SecVal[i] = 1.0 + C[i] * Z1 + C[J+i] * Z2;
	}

	return R;
}


vector<double> GetGrad
(
	double H0,
	const vector<double> &B,
	bool isNumDeg,
	const vector<double> &A,
	bool isDenDeg,

	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,

	int p,
	bool isRoughOF
)
{
	size_t J1 = B.size() >> 1;
	size_t J2 = A.size() >> 1;

	size_t len1 = isNumDeg ? J1-1 : J1;
	size_t len2 = isDenDeg ? J2-1 : J2;
	size_t len = 1 + J1+len1 + J2+len2;

	//Initialize output gradient vector with zeros
		vector<double> g(len);
	//Auxiliary vectors (values of particular sections on specified frequency)
		vector<complex<double>> NumSecVal(J1), DenSecVal(J2);
	//Auxiliary scalars
		complex<double> z, P, Q;
		double w, D, M, absP, absQ;
		double absH0 = abs(H0), signH0 = sign(H0);
	
	size_t GridLen = z_grid.size();
	for (uint zInd = 0; zInd < GridLen; zInd++)
	//Loop over frequencies.
	//On each iteration gradient obtain new term
	{
		//Initialize input variables for dFI_dx
			//z-argument and desired value
				z = z_grid[zInd];
				M = M_grid[zInd];
			//Compute P, Q, NumSecVal and DenSecVal, absP, absQ
				P = PolyVal(B, z,   NumSecVal);
				Q = PolyVal(A, z,   DenSecVal);
				absP = abs(P);
				absQ = abs(Q);
			//Compute D
				if (!isRoughOF)
					D = absH0 * absP / absQ - M;
				else
					D = absH0 * absP - M * absQ;

		w = w_grid[zInd];

		//Compute gradient vector
			g[0] += w * dFI_dx('H', 0, 0, z,   p, isRoughOF, M, D, P, Q, NumSecVal, DenSecVal, absH0, signH0, absP, absQ);
			for (uint i = 0; i < J1; i++)
				g[1           +i] += w * dFI_dx('b', i, 1, z,   p, isRoughOF, M, D, P, Q, NumSecVal, DenSecVal, absH0, signH0, absP, absQ);
			for (uint i = 0; i < len1; i++)
				g[1+J1        +i] += w * dFI_dx('b', i, 2, z,   p, isRoughOF, M, D, P, Q, NumSecVal, DenSecVal, absH0, signH0, absP, absQ);
			for (uint i = 0; i < J2; i++)
				g[1+J1+len1   +i] += w * dFI_dx('a', i, 1, z,   p, isRoughOF, M, D, P, Q, NumSecVal, DenSecVal, absH0, signH0, absP, absQ);
			for (uint i = 0; i < len2; i++)
				g[1+J1+len1+J2+i] += w * dFI_dx('a', i, 2, z,   p, isRoughOF, M, D, P, Q, NumSecVal, DenSecVal, absH0, signH0, absP, absQ);
	}

	return g;
}

//	Compose vector of unknowns from coefficients of SOS-polynomials and gain
vector<double> HBA2x
(	
	double H0,
	const vector<double> &B,
	bool isNumDeg,
	const vector<double> &A,
	bool isDenDeg
)
{
	size_t J1 = B.size() >> 1;
	size_t J2 = A.size() >> 1;

	size_t len1 = J1;
	size_t len2 = J2;

	if (isNumDeg)
		len1--;
	if (isDenDeg)
		len2--;

	vector<double> x(1+J1+len1+J2+len2);

	x[0] = H0;
	if (J1 != 0)
		wmemcpy( (wchar_t*) &x[1],         (wchar_t*) &B[0], (J1+len1)*sizeof(double)/2 );
	if (J2 != 0)
		wmemcpy( (wchar_t*) &x[1+J1+len1], (wchar_t*) &A[0], (J2+len2)*sizeof(double)/2 );

	return x;
}

vector<double> SymMatVecProd
(
	const vector<double> &A,
	const vector<double> &x
)
{
	size_t len = x.size();
	
	vector<double> b(len);

	for (uint i = 0; i < len; i++)
	//Loop over rows of A
	{
		for (uint k = 0; k < i; k++)
			b[i] += A[k*len+i] * x[k];
		for (uint k = i; k < len; k++)
			b[i] += A[i*len+k] * x[k];
	}

	return b;
}

void GetInvHesEst
(
	double H0,
	const vector<double> &B,
	bool isNumDeg,
	const vector<double> &A,
	bool isDenDeg,

	vector<double> &S,
	vector<double> &x_old,
	const vector<double> &g,
	vector<double> &g_old,
	bool isFirstIter,

	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	bool isRoughOF
)
{
	//Prepare vector of variables
		vector<double> x = HBA2x(H0, B, isNumDeg, A, isDenDeg);
	
	size_t len = x.size();

	if (isFirstIter)
	//This is the first iteration of quasi-Newton algorithm.
	//Assign identity matrix to S
	//(S is already initialized with zeros in DoOptimization function)
	{
		for (uint i = 0; i < len; i++)
			S[i*len+i] = 1;
	}
	else
	{
		vector<double> delta(len);
		vector<double> gamma(len);

		//Auxiliary variables
		double c1 = 0;
		double c2 = 0;
		//Auxiliary symmetric matrixes (only upper triangular parts are computed)
		vector<double> M1(len*len), M4(len*len);

		double M4_term1, M4_term2;
		
		double c2_term;

		//Compute delta and gamma
		for (uint i = 0; i < len; i++)
		{
			delta[i] = x[i] - x_old[i];
			gamma[i] = g[i] - g_old[i];
		}

		//Compute c1, c2, M1 and M2 (the last one is computed only for BFGS method)
		for (uint i = 0; i < len; i++)
		{
			c1 += gamma[i] * delta[i];
			c2_term = 0;
			for (uint j = 0; j < i; j++)
			{
				c2_term += S[j*len+i] * gamma[j];
			}
			for (uint j = i; j < len; j++)
			{
				c2_term += S[i*len+j] * gamma[j];
				M1[i*len+j] = delta[i] * delta[j];
			}
			c2 += gamma[i] * c2_term;
		}

		//Compute matrix M4
		for (uint i = 0; i < len; i++)
			for (uint j = i; j < len; j++)
			{
					M4_term1 = 0;
					M4_term2 = 0;
					for (uint k = 0; k < i; k++)
						M4_term1 += S[k*len+i] * gamma[k];
					for (uint k = i; k < len; k++)
						M4_term1 += S[i*len+k] * gamma[k];
					for (uint k = 0; k < j; k++)
						M4_term2 += S[k*len+j] * gamma[k];
					for (uint k = j; k < len; k++)
						M4_term2 += S[j*len+k] * gamma[k];
					M4[i*len+j] = M4_term1 * M4_term2;
			}

		//Compute matrix S
		for (uint i = 0; i < len; i++)
			for (uint j = i; j < len; j++)
				S[i*len+j] += M1[i*len+j] / c1 - M4[i*len+j] / c2;
	}

	g_old = g;
	x_old = x;
}
void x2HBA
(
	const vector<double> &x,
	
	double &H0,
	vector<double> &B,
	bool isNumDeg,
	vector<double> &A,
	bool isDenDeg
)
{
	size_t J1 = B.size() >> 1;
	size_t J2 = A.size() >> 1;

	size_t len1 = J1;
	size_t len2 = J2;

	if (isNumDeg)
		len1--;
	if (isDenDeg)
		len2--;

	H0 = x[0];
	if (J1 != 0)
		wmemcpy( (wchar_t*) &B[0], (wchar_t*) &x[1]        , (J1+len1)*sizeof(double)/2 );
	if (J2 != 0)
		wmemcpy( (wchar_t*) &A[0], (wchar_t*) &x[1+J1+len1], (J2+len2)*sizeof(double)/2 );

	//(Degenerated coefficients are already marked up with zero)
}

complex<double> PolyVal
(
	const vector<double> &C,
	complex<double> z
)
{		
	size_t J = C.size() >> 1;

	complex<double> R = 1;

	if (J != 0)
	{
		complex<double> Z1, Z2;
		Z1 = 1.0 / z;
		Z2 = Z1 * Z1;
		
		for (uint i = 0; i < J; i++)
		//Loop over the sections.
		//(Degenerated coefficient is marked up with 0)
			R *= 1.0 + C[i] * Z1 + C[J+i] * Z2;
	}

	return R;
}

double ObjFun
(
	double H0,
	const vector<double> &B,
	const vector<double> &A,

	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	int p,
	bool isRoughOF
)
{
	complex<double> P, Q;
	double D, absH0 = abs(H0);
	double err = 0;

	size_t GridLen = z_grid.size();

	for (uint i = 0; i < GridLen; i++)
	//Loop over frequency values
	{
		P = PolyVal(B, z_grid[i]);
		Q = PolyVal(A, z_grid[i]);

		if ( !isRoughOF )
			D = absH0 * abs( P ) / abs( Q ) - M_grid[i];
		else
			D = absH0 * abs( P ) - M_grid[i] * abs( Q );

		err += w_grid[i] * pow(D, p);
	}

	return err;
}

double v2err
(
	double v,
	const vector<double> &x,
	const vector<double> &dir,

	size_t J1,
	size_t J2,
	bool isNumDeg,
	bool isDenDeg,
	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	int p,
	bool isRoughOF
)
{
	//Make a copy of constant variable
		vector<double> x_copy = x;

	size_t len = x.size();
	for (uint i = 0; i < len; i++)
		x_copy[i] -= v * dir[i];
	
	double H0;
	vector<double> B(2*J1), A(2*J2);
	//(Degenerated coefficients are marked up with zero)

	x2HBA(x_copy,   H0, B, isNumDeg, A, isDenDeg);
	
	double err = ObjFun(H0, B, A,   z_grid, M_grid, w_grid, p, isRoughOF);

	return err;
}

void SimplexStep
(
	vector<double> &x,
	double &v1,
	double &v2,
	double &err1,
	double &err2,
	
	const vector<double> &dir,

	size_t J1,
	size_t J2,
	bool isNumDeg,
	bool isDenDeg,
	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	int p,
	bool isRoughOF
)
{
	double v = (v1 + v2) / 2;
	double err = v2err(v,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);

	if (err1 > err && err < err2)
	//Contract interval by half
		if (err1 < err2)
		{
			v2 = v;
			err2 = err;
		}
		else
		{
			v1 = v;
			err1 = err;
		}
	else if (err1 > err && err > err2)
	//Shift interval by half towards inf and, if this is insufficient, double the interval
	{
		v1 = v;
		err1 = err;
		//v = v2;
		err = err2;
		v2 = v2 + (v2-v1);
		//Compute err2
			err2 = v2err(v2,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
		if (err1 > err && err > err2)
        //Double interval towards inf
		{
            v2 = v2 + (v2-v1);
            //Compute err2
                err2 = v2err(v2,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
        }
	}
	else if (err1 < err && err < err2)
	//Shift interval by half towards 0.
	//If v1 < 0 assign v1 = 0
	{
		v2 = v;
		err2 = err;
		v1 = v1 - (v2-v1);
        if (v1 < 0)
            v1 = 0;
		//Compute err1
			err1 = v2err(v1,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
	}
}

double LineSearch
(
	double &H0,
	vector<double> &B,
	bool isNumDeg,
	vector<double> &A,
	bool isDenDeg,

	const vector<double> &delta,
	const vector<double> &g,
	const SLineSearchStopConds &StopConds,

	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	int p,
	bool isRoughOF,

	double &dX
)
{
	int IterLimit = StopConds.IterLimit;
	double dX_st = StopConds.dX_st;
	double dX_tol = StopConds.dX_tol;

	//Initial ranges of search interval
		double v1 = 0;
		vector<double> dir = delta;
        double NormDir = norm(dir);
        double v2 = dX_st / NormDir;
		
	vector<double> x = HBA2x(H0, B, isNumDeg, A, isDenDeg);	
	
	size_t J1 = B.size() >> 1;
	size_t J2 = A.size() >> 1;

	//Decide if gradient direction will be used for line search instead of Newton direction
		double err1 = v2err(v1,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
		double err2 = v2err(v2,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
		if (err1 < err2)
		//Line search will be done along the gradient direction
		//(Ineffectiveness of Newton direction is probably caused by negative definitness of Hessian)
		{
            dir = g;
            NormDir = norm(dir);
            v2 = dX_st / NormDir;
            err2 = v2err(v2,   x, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
		}
	
	double dX_old = dX_st;
	int iter = 1;
	while (true)
	//For loop over iterations is unacceptable because IterLimit might be == 0
	//(there are no restriction on maximum iteration number it that case)
	{
		SimplexStep(x, v1, v2, err1, err2, dir,   J1, J2, isNumDeg, isDenDeg, z_grid, M_grid, w_grid, p, isRoughOF);
		dX = NormDir * (v2-v1);

		if (dX <= dX_old && dX < dX_tol)
        //Desired tolerance is reached
            break;
        if (iter == IterLimit)
        //Maximum iteration number is reached
			break;

        dX_old = dX;
		iter++;
	}
	
	double v = (v1 + v2) / 2;
	
	size_t len = x.size();
	for (uint i = 0; i < len; i++)
		x[i] -= v * dir[i];
	
	x2HBA(x,   H0, B, isNumDeg, A, isDenDeg);
	
	//Compute Euclidean norm of the step vector
		dX = v * NormDir;
		
	//Compute reached value of objective double
		double err = ObjFun(H0, B, A,   z_grid, M_grid, w_grid, p, isRoughOF);

	return err;
}

double QuasiNewtonStep
(
	double &H0,
	vector<double> &B,
	bool isNumDeg,
	vector<double> &A,
	bool isDenDeg,
	
	double gradX_tol,
	const SLineSearchStopConds &LineSearchStopConds,

	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	int p,
	bool isRoughOF,
	bool isFirstIter,

	vector<double> &S,
	vector<double> &x_old,
	vector<double> &g_old,
	double &normG,
	double &dX
)
{
	//Compute gradient vector
		vector<double> g = GetGrad(H0, B, isNumDeg, A, isDenDeg,   z_grid, M_grid, w_grid, p, isRoughOF);

	//Compute norm of gradient vector
		normG = norm(g);

	if (normG < gradX_tol)
	//Gradient norm tolerance is reached
		return -1;

	//Compute estimation of inverse Hessian
	//(only upper triangular part of symmetric matrix is computed)
		GetInvHesEst(H0, B, isNumDeg, A, isDenDeg,   S, x_old, g, g_old,   isFirstIter, z_grid, M_grid, w_grid, isRoughOF);

	//Compute delta vector: delta = S * g
	//(S symmetry is taken into account)
		vector<double> delta = SymMatVecProd(S, g);

	//Line-search stage: optimize multiplier of delta- or g-vector and do the step
		double err;
		err = LineSearch(H0, B, isNumDeg, A, isDenDeg,   delta, g, LineSearchStopConds, z_grid, M_grid, w_grid, p, isRoughOF,   dX);

	return err;
}

vector<complex<double>> SecPolyCoefs2Roots
(
	const vector<double> &C,
	bool isDeg
)
{
	double b, c;
	complex<double> d, r1, r2;

	size_t J = C.size() >> 1;

	int Nr = 2*J;
	if (isDeg)
		Nr--;

	vector<complex<double>> r(Nr);

	int end = !isDeg ? J : J-1;
	complex<double> sqrtd;
	for (int i = 0; i < end; i++)
	//Loop over the non-degenerated sections (having 2 roots)
	{
		b = C[i];
		c = C[J+i];

		d = b*b - 4*c;
		sqrtd = sqrt(d);
		r1 = ( -b + sqrtd ) / 2.0;
		r2 = ( -b - sqrtd ) / 2.0;
		r[2*i  ] = r1;
		r[2*i+1] = r2;
	}

	if (isDeg)
	//The last section is degenerated (it has only 1 root)
	{
		b = C[J-1];
		r[Nr-1] = -b;
	}

	return r;
}


double PolesZerosProc_aux
(
	vector<double> &C,
	bool isDeg,
	bool isReflection,
	double MaxRad
)
{
	size_t J = C.size() >> 1;

	double AbsGainMult = 1;
	double a, b;

	if (isReflection)
	{
		//Calculate all the roots of section-form polynomial
			vector<complex<double>> r = SecPolyCoefs2Roots(C, isDeg);

		size_t ord = r.size();
		for (uint i = 0; i < ord; i++)
		//Loop over the roots
		{
			if ( abs( r[i] ) > 1 )
			//Reflect the root into the unit circle and
			//adjust polynomial gain multiplier
			//( conj() is ommitted because all complex roots have conjugated counterpart )
			{
				AbsGainMult *= abs( r[i] );
				r[i] = 1.0 / r[i];
			}

			if ( abs( r[i] ) > MaxRad )
			//Restrict root radius value
			{
				a = abs( r[i] ) / MaxRad;
				b = a * ( a + pow( abs(r[i]), 2 ) ) / ( pow(a, 2) + pow( abs(r[i]), 2 ) );
				r[i] /= a;
				//Adjust gain multiplier so that
				//
				//  int ( |(z-r) - b(z-r/a)|^2 ) dz -> min
				// |z|=1
				//
					AbsGainMult *= b;
			}
		}

		//Convert the roots to coefficients of SOS-polynomial
			C = Roots2SecPolyCoefs(r, isDeg);
	}
	return AbsGainMult;
}

void PolesZerosProc
(
	double &H0,
	vector<double> &B,
	bool isNumDeg,
	vector<double> &A,
	bool isDenDeg,
	const SPolesZerosProcOpts &opts
)
{
	size_t J1 = B.size() >> 1;
	size_t J2 = A.size() >> 1;

	bool isPolesReflection = opts.isPolesReflection;
	bool isZerosReflection = opts.isZerosReflection;
	double MaxPoleRad = opts.MaxPoleRad;
	double MaxZeroRad = opts.MaxZeroRad;

	//Poles processing
		double AbsDenGainMult = PolesZerosProc_aux(A, isDenDeg, isPolesReflection, MaxPoleRad);

	//Zeros processing
		double AbsNumGainMult = PolesZerosProc_aux(B, isNumDeg, isZerosReflection, MaxZeroRad);
   
	//Adjust filter gain
	//(absolute value is given for the sake of minimization of H0 variation)
		H0 = H0 * AbsNumGainMult / AbsDenGainMult;
}

double DoOptimization
(
	double &H0,
	vector<double> &B,
	bool isNumDeg,
	vector<double> &A,
	bool isDenDeg,

	int p,
	bool isRoughOF,

	const SPolesZerosProcOpts &PoleZeroOpts,
	const SNewtonStopConds &NewtonStopConds,
	const SLineSearchStopConds &LineSearchStopConds,
	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
	
	int &res,
	std::string &msg
)
{
	double err;

	//Specify conditions of optimization breaking
		double Y_tol = NewtonStopConds.Y_tol;
		double gradX_tol = NewtonStopConds.gradX_tol;
		double dX_tol = NewtonStopConds.dX_tol;
		double dY_tol = NewtonStopConds.dY_tol;
		int IterLimit = NewtonStopConds.IterLimit;
		int TimeLimit = NewtonStopConds.TimeLimit;
		
	double dX = inf, normG, err_old = inf;
	int iter = 1;

	bool isRadRestr = ( PoleZeroOpts.isPolesReflection && PoleZeroOpts.MaxPoleRad != 1 ) ||
					  ( PoleZeroOpts.isZerosReflection && PoleZeroOpts.MaxZeroRad != 1 )    ;

	//The S matrix (estimation of inverse Hessian)
		size_t len = 1 + B.size() + A.size();
		if (isNumDeg)
			len--;
		if (isDenDeg)
			len--;
		vector<double> S(len*len);
	//Gradient and vector of variables corresponding to preceding iteration
		vector<double> g_old(len), x_old(len);

	bool isFirstIter = true;

	while (true)
	//Loop over optimization iterations
	{

		//Do the step of optimization using Newton method
			err =
				QuasiNewtonStep
				(
					H0,
					B,
					isNumDeg,
					A,
					isDenDeg,

					gradX_tol,
					LineSearchStopConds,

					z_grid,
					M_grid,
					w_grid,
					p,
					isRoughOF,
					isFirstIter,

					S,
					x_old,
					g_old,

					normG,
					dX
				);

			if (err == -1)
			//Gradient norm tolerance has been reached so the step was skipped
			{
				err = err_old;
			}

			isFirstIter = false;
		
		if (isRadRestr)
		//Poles and zeros processing
		{
			PolesZerosProc( H0, B, isNumDeg, A, isDenDeg,   PoleZeroOpts );
			//Recalculate objective function value
				err = ObjFun(H0, B, A, z_grid, M_grid, w_grid, p, isRoughOF);
		}
					
		iter++;

		res = 0;
		//Check stop conditions
			if (err < Y_tol)
			{
				msg = "Objective function tolerance reached.";
				res = 1;
			}
			else if (normG < gradX_tol)
			{
				msg = "Gradient norm tolerance reached.";
				res = 2;
			}
			else if (dX < dX_tol)
			{
				msg = "Step vector norm tolerance reached.";
				res = 3;
			}
			else if ( abs(err_old - err) < dY_tol )
			//abs() is necessary because radii restriction stage can lead to negative error decrement
			{
				msg = "Objective function decrement tolerance reached.";
				res = 4;
			}
			else if (IterLimit != 0 && iter > IterLimit)
			{
				msg = "Iteration limit reached.";
				res = -1;
			}

			if (res != 0)
				return err;

		err_old = err;
	}
}

vector<double> poly
(
	vector<complex<double>> r
)
{
	uint n = r.size();

	vector<complex<double>> cc(n+1);
	cc[0] = 1;

	for (uint j = 0; j < n; j++)
		for (int k = j; k >= 0; k--)	//Don't use uint! (see cycle stop condition)
			cc[k+1] -= r[j] * cc[k];

	vector<double> c(n+1);

	for (uint i = 0; i < n+1; i++)
	//Throw out possible imaginary parts caused by finite calculation precision
		c[i] = cc[i].real();

	return c;
}

void SecPolyCoefs2ExtPolyCoefs_aux
(
	const vector<double> &C,
	bool isDeg,

	vector<double> &c
)
{
	//Calculate all the roots of SOS-polynomial
		vector<complex<double>> r = SecPolyCoefs2Roots(C, isDeg);
		
	//Obtain polynomial coefficients
		c = poly(r);
}

void SecPolyCoefs2ExtPolyCoefs
(
	const double &H0,
	const vector<double> &B,
	bool isNumDeg,
	const vector<double> &A,
	bool isDenDeg,

	vector<double> &b,
	vector<double> &a
)
{
	SecPolyCoefs2ExtPolyCoefs_aux(B, isNumDeg, b);
	uint len = b.size();
	for (uint i = 0; i < len; i++)
		b[i] *= H0;
	SecPolyCoefs2ExtPolyCoefs_aux(A, isDenDeg, a);
}

double iirlpnorm_core(
	unsigned int NumOrd,
	unsigned int DenOrd,
	vector<double> &b,
	vector<double> &a,
	int p,

	const SPolesZerosProcOpts &PolesZerosProcOpts,
	bool isPreOptimization,
	const SNewtonStopConds &NewtonStopConds,
	const SLineSearchStopConds &LineSearchStopConds,

	const vector<complex<double>> &z_grid,
	const vector<double> &M_grid,
	const vector<double> &w_grid,
		double *ZRe,
        double *ZIm,
        double *PRe,
        double *PIm,
	int &res,
	std::string &msg
)
{

	//Number of second order sections in numerator and denomenator
		size_t J1 = (NumOrd+1) >> 1;
		size_t J2 = (DenOrd+1) >> 1;
	//Flags specifying whether last sections of SOS-polynomials will be degenerated
		bool isNumDeg = NumOrd & 1;
		bool isDenDeg = DenOrd & 1;
	//Length of variables vector
		size_t len = 1 + 2*J1 + 2*J2;
		if (isNumDeg)
			len--;
		if (isDenDeg)
			len--;

	//Prepare variables for SOS-polynomials parameters
	//(Degenerated coefficients are marked up with 0)
		double H0;
		vector<double> B(2*J1), A(2*J2);

	//Convert the cefficients of extended polynomials to the coefficients of SOS-polynomials and gain
		ExtPolyCoefs2SecPolyCoefs( b, a,   H0, B, A );

	bool isPureReflection = ( PolesZerosProcOpts.isPolesReflection && PolesZerosProcOpts.MaxPoleRad == 1 ) ||
							( PolesZerosProcOpts.isZerosReflection && PolesZerosProcOpts.MaxZeroRad == 1 )	  ;

	bool isRoughOF;
	double err;

	if (isPreOptimization)
	//Optimize simplified objective function of type
	//	      N-1	    K-1
	//	OFs = sum ( w * sum ( |H0 * P  | - |Tlin  * Q  | ) ^ p )
	//	      i=0    i  j=0	         ij         ij   ij
	{
		isRoughOF = true;
		err =
			DoOptimization
			(
				H0,
				B,
				isNumDeg,
				A,
				isDenDeg,

				p,
				isRoughOF,

				PolesZerosProcOpts,
				NewtonStopConds,
				LineSearchStopConds,

				z_grid,
				M_grid,
				w_grid,

				res,
				msg
			);

		if (isPureReflection)
		//Do the reflection of poles and/or zeros
			PolesZerosProc( H0, B, isNumDeg, A, isDenDeg,   PolesZerosProcOpts );
	}

	if (res != -2)
	//TimeLimit hasn't been exceeded yet.
	//Optimize exact objective function of type
	//	     N-1       K-1
	//	OF = sum ( w * sum ( |T  | - |Tlin  | ) ^ p )
	//	     i=0    i  j=0     ij         ij
	{
		isRoughOF = false;
		err =
		DoOptimization
		(
			H0,
			B,
			isNumDeg,
			A,
			isDenDeg,

			p,
			isRoughOF,

			PolesZerosProcOpts,
			NewtonStopConds,
			LineSearchStopConds,

			z_grid,
			M_grid,
			w_grid,

			res,
			msg
		);

		if (isPureReflection)
		//Do the reflection of poles and/or zeros
			PolesZerosProc( H0, B, isNumDeg, A, isDenDeg,   PolesZerosProcOpts );
	}

	//Convert the cefficients of SOS-polynomials and gain to the coefficients of extended polynomials
		SecPolyCoefs2ExtPolyCoefs( H0, B, isNumDeg, A, isDenDeg,   b, a );
		vector<complex<double>> rz = SecPolyCoefs2Roots(B, B[B.size()-1] == 0);
		vector<complex<double>> rp = SecPolyCoefs2Roots(A, A[A.size()-1] == 0);
		for (int i = 0; i < rz.size(); i++)
		{
			ZRe[i] = rz[i].real();
			ZIm[i] = rz[i].imag();
		}
		for (int i = 0; i < rp.size(); i++)
		{
			PRe[i] = rp[i].real();
			PIm[i] = rp[i].imag();
		}
	return err;
}


double iirlpnorm(
	unsigned int NumOrd,
	unsigned int DenOrd,
	size_t FreqNum,
	double *f,
	double *M,
	double *b,
	double *a,
	unsigned int p,
	size_t EdgesNum,
	double *edges,
	size_t WeigNum,
	double *w,
	size_t PtsPerIntNum,
	SPolesZerosProcOpts *PolesZerosProcOpts,
	bool isPreOptimization,
	SNewtonStopConds *NewtonStopConds,
	SLineSearchStopConds *LineSearchStopConds,
		double *ZRe,
        double *ZIm,
        double *PRe,
        double *PIm,
	int *res,
	std::string *msg)
{
	int res_ = 0;
	std::string msg_;

	//Convert arrays b and a to vectors, assign initial values
		vector<double> vb(&b[0], &b[NumOrd+1]), va(&a[0], &a[DenOrd+1]);
		
	//Choose initial values of polynomials coefficients.
	//The coefficients aren't redefined if they've already specified
		GetInitCoefs
		(
			NumOrd,
			DenOrd,
			PolesZerosProcOpts->MaxPoleRad,
			PolesZerosProcOpts->MaxZeroRad,

			vb,
			va
		);

	//Prepare arrays of z, M and w according to specified PtsPerIntNum value.
	//Non-optimizable intervals are excluded from the grids.
		vector<complex<double>> z_grid;
		vector<double> M_grid, w_grid;
		GetGrids(FreqNum, f, EdgesNum, edges, M, WeigNum, w, PtsPerIntNum,   z_grid, M_grid, w_grid);

	double err = iirlpnorm_core
	(
		NumOrd,
		DenOrd,
		vb,
		va,
		(int)p,

		*PolesZerosProcOpts,
		isPreOptimization,
		*NewtonStopConds,
		*LineSearchStopConds,
		z_grid,
		M_grid,
		w_grid,
		ZRe,
        ZIm,
        PRe,
        PIm,
		res_,
		msg_
	);

	//Convert vectors bv and av to native arrays
		memcpy( b, &vb[0], (NumOrd+1)*sizeof(double) );
		memcpy( a, &va[0], (DenOrd+1)*sizeof(double) );

	if (res != NULL)
		*res = res_;
	if (msg != NULL)
		*msg = msg_;

	return err;

}



//iirlpn(new WraperISCMS.CallBackCancel(bw.IsCancelChaeck), ((Int32)Order), ((Int32)OrderDen), edgeFqs, edges, des_mag, weights, NumCoef, DenCoef, pzArray.ZRe, pzArray.ZIm, pzArray.PRe, pzArray.PIm, ref pzArray.ggain);
extern "C" __declspec(dllexport)
int iirlpn( IS_CANCEL IsCancel,
              int NumOrd,
              int DenOrd,
              double *f,
              double *edges,			  
              double *M,
              double *w,
              size_t FreqNum,
              size_t EdgesNum,
              size_t WeigNum,
			  double *num,
              double *den,
			  double ZRe[],
              double ZIm[],
              double PRe[],
              double PIm[],
              int p,
			  double *ga
              )
{
	*ga = 1.0;
	vector<double> b(NumOrd+1);
    b[0] = nan;
	vector<double> a(DenOrd+1);
	a[0] = nan;

	size_t PtsPerIntNum = 20;
	SPolesZerosProcOpts PolesZerosProcOpts = {true, true, 1, 1};
	bool isPreOptimization = false;
	SNewtonStopConds NewtonStopConds = {1e-7, 1e-3, 1e-4, 1e-6, 40, 0};
	SLineSearchStopConds LineSearchStopConds = {1e-6, 1e-7, 30};

	int res;
    std::string msg;

	double err = iirlpnorm
	(
		NumOrd,
		DenOrd,
		FreqNum,
		f,
		M,
		&b[0],
		&a[0],
		p,
		EdgesNum,
		edges,
		WeigNum,
		w,
		PtsPerIntNum,
		&PolesZerosProcOpts,
		isPreOptimization,
		&NewtonStopConds,
		&LineSearchStopConds,
		ZRe,
        ZIm,
        PRe,
        PIm,
		&res,
		&msg
	);
	memcpy(num, b.data(), (NumOrd+1)*sizeof(double));
	memcpy(den, a.data(), (DenOrd+1)*sizeof(double));
    return 0;
}
