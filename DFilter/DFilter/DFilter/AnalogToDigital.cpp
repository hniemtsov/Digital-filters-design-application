//#include "ipp.h"
#include <stdlib.h>
#include <limits>
#include "math.h"
#include <time.h>
#include <memory.h>
#include "type_utils.h"

typedef struct _my64fc
{
    double re;
    double im;
}MY64FC;

void mySub_64fc(const MY64FC *pSrc, MY64FC *pDst, int len)
{
    int i;
    for (i=0;i<len;i++)
    {
        pDst[i].re = pDst[i].re - pSrc[i].re;
        pDst[i].im = pDst[i].im - pSrc[i].im;
    }
    return;
}

void myMulC_64fc(const MY64FC *pSrc, MY64FC val, MY64FC *pDst, int len)
{
    int i;
    MY64FC tmp;
    for (i = 0; i < len; i++)
    {
        tmp.re = pSrc[i].re * val.re - pSrc[i].im * val.im;
        tmp.im = pSrc[i].re * val.im + pSrc[i].im * val.re;
        pDst[i].re = tmp.re;
        pDst[i].im = tmp.im;
    }
    return;
}

void myDivC_64fc(MY64FC val, MY64FC *pS)
{
    double absval = val.re*val.re + val.im*val.im;
    MY64FC tmp;
    tmp.re = (val.re*pS[0].re+val.im*pS[0].im)/absval;
    tmp.im = (val.re*pS[0].im-val.im*pS[0].re)/absval;
    pS[0].re = tmp.re;
    pS[0].im = tmp.im;

}

void mySqrt_64fc(MY64FC *val)
{
    double abs = sqrt(sqrt(val[0].re*val[0].re + val[0].im*val[0].im));
    double angle = atan2(val[0].im,val[0].re)/2.0;
    val[0].re = abs*cos(angle);
    val[0].im = abs*sin(angle);
    return;
}

double asinh(double x)
{
	return log(x + sqrt(x*x + 1));
}

int LP2BS(MY64FC *X,
            int nX,    //         s * B
            double A,  //  s -> ---------
            double B)  //        s^2 + A
{
    int i;
    MY64FC *pEnd = X + nX;
    MY64FC D, Bs1;
    for (i = 0; i < nX; i++)
    {
        Bs1.re = (B/(X[i].re*X[i].re + X[i].im*X[i].im))*X[i].re;
        Bs1.im =-(B/(X[i].re*X[i].re + X[i].im*X[i].im))*X[i].im;
        myMulC_64fc(&Bs1, Bs1, &D, 1);
        D.re -= 4*A;
        mySqrt_64fc(&D);
        X[i].re = (Bs1.re + D.re)/2;
        X[i].im = (Bs1.im + D.im)/2;
        pEnd[i].re = (Bs1.re - D.re)/2;//X[i].re;
        pEnd[i].im = (Bs1.im - D.im)/2;
    }
    return (2*nX);
}

int LP2BP(MY64FC *X,
            int nX,    //        s^2 + A
            double A,  //  s -> ---------
            double B)  //          s*B
{
    int i;
    MY64FC *pEnd = X + nX;
    MY64FC D, Bs1;
    for (i = 0; i < nX; i++)
    {
        Bs1.re = B*X[i].re;
        Bs1.im = B*X[i].im;
        myMulC_64fc(&Bs1, Bs1, &D, 1);
        D.re -= 4*A;
        mySqrt_64fc(&D);
        X[i].re = (Bs1.re + D.re)/2;
        X[i].im = (Bs1.im + D.im)/2;
        pEnd[i].re = (Bs1.re - D.re)/2;//X[i].re;
        pEnd[i].im = (Bs1.im - D.im)/2;
    }
    return (2*nX);
}

void ZP_LP2BP(MY64FC *Z,
            int *nZ,
            MY64FC *P,
            int *nP,
            int *ZZ,    //power of zero zero
            int *ZP,    //power of zero pole
            MY64FC *G, // gain
            double Wc1,
            double Wc2)
{
    double A = Wc1 * Wc2;
    double B = Wc2 - Wc1;
    int N =  (*nP) - (*nZ);

    (*G).re *= pow(B, N);
    if (N < 0) (*ZP) -= N; else (*ZZ) += N;

    (*nZ) = LP2BP(Z, *nZ, A, B);
    (*nP) = LP2BP(P, *nP, A, B);

    return;
}

void ZP_LP2BS(MY64FC *Z,
            int *nZ,
            MY64FC *P,
            int *nP,
            int *ZZ,    //power of zero zero
            int *ZP,    //power of zero pole
            MY64FC *G, // gain
            double Wc1,
            double Wc2)
{
    double A = Wc1 * Wc2;
    double B = Wc2 - Wc1;
    int N =  (*nP) - (*nZ);
    MY64FC *X;
    double tmp;
    int i;


    if ((N & 0x01) == 1) (*G).re = -(*G).re;

    for (i = 0; i < (*nP); i++)
    {
        myDivC_64fc(P[i], G);
    }
    for (i = 0; i < (*nZ); i++)
    {
        myMulC_64fc(G, Z[i], G, 1);
    }

    (*nZ) = LP2BS(Z, *nZ, A, B);
    (*nP) = LP2BS(P, *nP, A, B);

    if (N < 0)
    {
        N = -N;
        X = &P[*nP];
        (*nP) += 2*N;
    }
    else
    {
        X = &Z[*nZ];
        (*nZ) += 2*N; 
    }
    tmp = sqrt(A);
    for (i = 0; i < N; i++)
    {
        (*X).re = 0;
        (*X).im = tmp;
        X++;
        (*X).re = 0;
        (*X).im = -tmp;
        X++;
    }

    return;
}

void ZP_LP2LP(MY64FC *Z,
            int *nZ,
            MY64FC *P,
            int *nP,
            int *ZZ,    //power of zero zero
            int *ZP,    //power of zero pole
            MY64FC *G, // gain
            double Wc)  //cutoff frequency
{
    double B = Wc;
    int N =  (*nP) + (*ZP) - (*nZ) - (*ZZ);
    int i;

    (*G).re *= pow(B, N);

    for (i = 0; i < (*nP); i++)
    {
        P[i].re = B*P[i].re;
        P[i].im = B*P[i].im;
    }
    for (i = 0; i < (*nZ); i++)
    {
        Z[i].re = B*Z[i].re;
        Z[i].im = B*Z[i].im;
    }
    return;
}

void ZP_LP2HP(MY64FC *Z,
            int *nZ,
            MY64FC *P,
            int *nP,
            int *ZZ,    //power of zero zero
            int *ZP,    //power of zero pole
            MY64FC *G, // gain
            double Wc)  //cutoff frequency
{
    double B = Wc;
    double tmp;
    int N =  (*nP) - (*nZ);
    int i;

    if (N & 0x01 == 1) (*G).re *= -1.;

    if (N < 0) (*ZP) -= N; else (*ZZ) += N;

    for (i = 0; i < (*nP); i++)
    {
        myDivC_64fc(P[i], G);
        tmp = B/(P[i].re*P[i].re + P[i].im*P[i].im);
        P[i].re =  tmp*P[i].re;
        P[i].im = -tmp*P[i].im;
    }
    for (i = 0; i < (*nZ); i++)
    {
        myMulC_64fc(G, Z[i], G, 1);
        tmp = B/(Z[i].re*Z[i].re + Z[i].im*Z[i].im);
        Z[i].re = tmp*Z[i].re;
        Z[i].im = -tmp*Z[i].im;
    }
    return;
}

int BILINEAR(MY64FC *X,  //         z - 1
             int nX,      //  s -> 2 -------
             MY64FC *G)  //         z + 1
{
    int i;
    MY64FC tmp;
    (*G).re = 1.0;
    (*G).im = 0.0;
    for (i = 0; i < nX; i++)
    {
        tmp.re = 1 - 0.5*X[i].re;
        tmp.im = - 0.5*X[i].im;
        myMulC_64fc(G, tmp, G, 1);

        X[i].re = 1.0 + 0.5*X[i].re; 
        X[i].im = 0.5*X[i].im; 
        myDivC_64fc(tmp, &X[i]);
    }
    return nX;
}

void ZP_s2z(MY64FC *Z,
            int *nZ,
            MY64FC *P,
            int *nP,
            MY64FC *G, // gain
            int *nZZ,
            int *nZP)
{
    MY64FC tmp1, tmp2;
    MY64FC *pTmp;
    int i, N1, N2;

    //to minimize round error can be sorted by value |1-s1|
//
    N1 = BILINEAR(P, *nP, &tmp2);
    N1 -= BILINEAR(Z, *nZ, &tmp1);
    
    myDivC_64fc(tmp2, &tmp1); //  (tmp1/tmp2)
    myMulC_64fc(G, tmp1, G, 1); // updated gain

    N2 = (*nZP);
    N2 -= (*nZZ);

    N1 += N2;
    (*G).re *= pow(2.0, -N1);
//
    if (N2 < 0)
    {
        N2 = -N2;
        pTmp = Z + (*nZ);
        (*nZ) += N2;
    }
    else
    {
        pTmp = P + (*nP);
        (*nP) += N2;
    }
    for (i = 0; i < N2; i++)
    {
        pTmp[i].re = 1.0;
        pTmp[i].im = 0.0;
    }
//
    if (N1 > 0)
    {
        pTmp = Z + (*nZ);
        (*nZ) += N1;
    }
    else
    {
        N1 = -N1;
        pTmp = P + (*nP);
        (*nP) += N1;
    }
    for (i = 0; i < N1; i++)
    {
        pTmp[i].re = -1.0;
        pTmp[i].im = 0.0;
    }

    (*nZZ) = 0;
    (*nZP) = 0;

    return;
}

void poly(MY64FC *InOut, int Len)
{
	int i;
	MY64FC *work = (MY64FC *)malloc(Len*sizeof(MY64FC));
    MY64FC *out  = (MY64FC *)malloc(Len*sizeof(MY64FC));
    memset(work, 0, Len*sizeof(MY64FC));
	memset(out, 0, Len*sizeof(MY64FC));
	out[0].re = 1;
    for (i = 0; i < Len-1; i++)
	{
		myMulC_64fc(out, InOut[i], work, i+1);
		mySub_64fc(work, out+1, i+1);
	}
    memcpy(InOut, out, Len*sizeof(MY64FC));
    free(work);
    free(out);
}
static double deps = std::numeric_limits<double>::epsilon();

extern "C" __declspec(dllexport)
int cheby1(IS_CANCEL IsCancel,
           int n,           // filter length
           double w1,       // band edge
           double w2,       // band edge w1 < w2
           double R,        // ripple
           double Num[],    // 
           double Den[],    //
           double ZRe[],
           double ZIm[],
           double PRe[],
           double PIm[],
           double *ga,
           int type)        // 0 - low, 1 - high, 2 - stop, 3 - pass;
{
    int r, i, i2, n2;
    MY64FC gain;
    double epsl, tmp1, tmp2;
    int nP, nZ, nZZ, nZP;
    int order = n - 1;
    MY64FC Z[100];
    MY64FC P[100];
    int Ret = 0;
    
    nZZ = nZP = 0; // count of the zero root
//-------=== Normalized System Function in s-plain ===----------            //
    n2 = n<<1;                                                              //
    nP = nZ = 0;                                                            //
    //if (1 == (n & 0x01)) r = order>>1; else r = n>>1;                       //
    r = n>>1;
    epsl = sqrt(pow(10, R/10)- 1);                                          //
    tmp1 = cosh(asinh(1/epsl)/(double)n);                                    //
    tmp2 = -sinh(asinh(1/epsl)/(double)n);
    if (1 == (n & 0x01))
    {
        P[0].re = tmp2;                               //
        P[0].im = 0;                                                            //
        nP++;
    }
    gain.re = 1.0;                                                          //
    gain.im = 0.0;
    i2 = 1;//
    for (i = nP; i < r + nP; i++)                                                //
    {                                                                       //
        P[i].re = tmp2*sin(i2*MY_PI/2/n);                          //
        P[i].im = tmp1*cos(i2*MY_PI/2/n);                              //
        P[r+i].re = P[i].re;                                                //
        P[r+i].im = -P[i].im;                                               //
        i2 += 2;                                                        //        
        gain.re *= (P[i].re*P[i].re + P[i].im*P[i].im);                     //
    }   
    nP += 2*r;                                                            //
    if (1 == (n & 0x01)) gain.re *= -P[0].re; else gain.re *= pow(10.0,-R/20.0);  //
//////////////////////////////////////////////////////////////////////////////
///// ------==== s-plain transformathions (analog -> analog) ===------ ///////
    switch(type)
    {
    case 0:
        ZP_LP2LP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5));
        break;
    case 1:
        ZP_LP2HP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5));
        break;
    case 2:
        ZP_LP2BP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5), 2.0*tan(MY_PI*w2*0.5));
        break;
    case 3:
        ZP_LP2BS(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5), 2.0*tan(MY_PI*w2*0.5));
        break;
    default:
        return -1;
        break;
    }
//////////////////////////////////////////////////////////////////////////////
/////// -----------==== transform: s-plain -> z-plain ===----------- /////////
    ZP_s2z(Z, &nZ, P, &nP, &gain, &nZZ, &nZP);
    *ga = gain.re;
    if (gain.im > deps)
    {
        //Ret = -1;
    }
    for (i = 0; i < nZ; i++)
    {
        ZRe[i] = Z[i].re;
        ZIm[i] = Z[i].im;
    }
    poly(Z, nZ+1);
    for (i = 0; i < nZ+1; i++)
    {
        Num[i] = Z[i].re*gain.re;
        if (Z[i].im > deps*abs(Z[i].re))
        {
            //Ret = -1;
        }
    }
    for (i = 0; i < nP; i++)
    {
        PRe[i] = P[i].re;
        PIm[i] = P[i].im;
    }
    poly(P, nP+1);
    for (i = 0; i < nP+1; i++)
    {
        Den[i] = P[i].re;
        if (P[i].im > deps*abs(P[i].re))
        {
            //Ret = -1;
        }
    }

    return Ret;
}

extern "C" __declspec(dllexport)
int cheby2(IS_CANCEL IsCancel,
           int n,           // filter length
           double w1,       // band edge
           double w2,       // band edge w1 < w2
           double R,        // ripple
           double Num[],    // 
           double Den[],    //
           double ZRe[],
           double ZIm[],
           double PRe[],
           double PIm[],
           double *ga,
           int type)        // 0 - low, 1 - high, 2 - stop, 3 - pass;
{
    int r, i, i2, n2;
    MY64FC gain;
    double delta, tmp1, tmp2, tmp3;
    int nP, nZ, nZZ, nZP;
    int order = n - 1;
    MY64FC Z[100];
    MY64FC P[100];
    int Ret = 0;
    
    nZZ = nZP = 0; // count of the zero root
//-------=== Normalized System Function in s-plain ===----------            //
    n2 = n<<1;                                                              //
    nP = nZ = 0;                                                            //
    //if (1 == (n & 0x01)) r = order>>1; else r = n>>1;                       //
    r = n>>1;
    delta = 1.0/sqrt(pow(10, R/10) - 1);                                          //
    tmp1 = cosh(asinh(1/delta)/(double)n);                                    //
    tmp2 = -sinh(asinh(1/delta)/(double)n);
    if (1 == (n & 0x01))
    {
        P[0].re = 1.0/tmp2;                               //
        P[0].im = 0;                                                            //
        nP++;
    }
    gain.re = 1.0;                                                          //
    gain.im = 0.0;
    i2 = 1;//
    for (i = nP; i < r + nP; i++)                                                //
    {                                                                       //
        tmp3 = tmp2*tmp2*sin(i2*MY_PI/2/n)*sin(i2*MY_PI/2/n)+tmp1*tmp1*cos(i2*MY_PI/2/n)*cos(i2*MY_PI/2/n); 
        P[i].re = tmp2*sin(i2*MY_PI/2/n)/tmp3;                          //
        P[i].im = -tmp1*cos(i2*MY_PI/2/n)/tmp3;                              //
        P[r+i].re = P[i].re;                                                //
        P[r+i].im = -P[i].im;                                               //
        
        Z[nZ].re = 0;
        Z[nZ].im = -1.0/cos(i2*MY_PI/2/n);
        Z[r+nZ].re = 0;
        Z[r+nZ].im = -Z[nZ].im;

        gain.re *= (cos(i2*MY_PI/2/n)*cos(i2*MY_PI/2/n))/tmp3;                     //
        i2 += 2;                                                        //        
        nZ++;
    }   
    nP += 2*r;    
    nZ += r;
    //
    if (1 == (n & 0x01)) gain.re *= -P[0].re; //else gain.re *= pow(10.0,-R/20.0);  //
//////////////////////////////////////////////////////////////////////////////
///// ------==== s-plain transformathions (analog -> analog) ===------ ///////
    switch(type)
    {
    case 0:
        ZP_LP2LP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5));
        break;
    case 1:
        ZP_LP2HP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5));
        break;
    case 2:
        ZP_LP2BP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5), 2.0*tan(MY_PI*w2*0.5));
        break;
    case 3:
        ZP_LP2BS(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5), 2.0*tan(MY_PI*w2*0.5));
        break;
    default:
        return -1;
        break;
    }
//////////////////////////////////////////////////////////////////////////////
/////// -----------==== transform: s-plain -> z-plain ===----------- /////////
    ZP_s2z(Z, &nZ, P, &nP, &gain, &nZZ, &nZP);
    *ga = gain.re;
    if (gain.im > deps)
    {
        //Ret = -1;
    }
    for (i = 0; i < nZ; i++)
    {
        ZRe[i] = Z[i].re;
        ZIm[i] = Z[i].im;
    }
    poly(Z, nZ+1);
    for (i = 0; i < nZ+1; i++)
    {
        Num[i] = Z[i].re*gain.re;
        if (Z[i].im > deps*abs(Z[i].re))
        {
            //Ret = -1;
        }
    }
    for (i = 0; i < nP; i++)
    {
        PRe[i] = P[i].re;
        PIm[i] = P[i].im;
    }
    poly(P, nP+1);
    for (i = 0; i < nP+1; i++)
    {
        Den[i] = P[i].re;
        if (P[i].im > deps*abs(P[i].re))
        {
            //Ret = -1;
        }
    }

    return Ret;
}

extern "C" __declspec(dllexport)
int butter(IS_CANCEL IsCancel,
           int n,           // filter length
           double w1,       // band edge
           double w2,       // band edge w1 < w2
           double Num[],    // 
           double Den[],    //
           double ZRe[],
           double ZIm[],
           double PRe[],
           double PIm[],
           double *ga,
           int type)        // 0 - low, 1 - high, 2 - stop, 3 - pass;
{
    int i, n2;
    MY64FC gain;
    double tmp1, tmp2, tmp3;
    int nP, nZ, nZZ, nZP;
    int order = n - 1;
    MY64FC Z[100];
    MY64FC P[100];
    int Ret = 0;
    
    nZZ = nZP = 0; // count of the zero root
//-------=== Normalized System Function in s-plain ===----------            //
    n2 = n<<1;                                                              //
    nP = nZ = 0;                                                            //
    tmp3 = (1 == (n & 0x01)) ? 1.0 : 0.5;                       //
    for (i = 1; i <= n2; i++)
    {
        tmp1 = cos((i-tmp3)*MY_PI/((double)n));                                    //
        tmp2 = sin((i-tmp3)*MY_PI/((double)n));                                    //
        if (tmp1 < 0)
        {
            P[nP].re = tmp1;
            P[nP].im = tmp2;
            nP++;
        }
    }
    gain.re = 1.0;                                                          //
    gain.im = 0.0;
//////////////////////////////////////////////////////////////////////////////
///// ------==== s-plain transformathions (analog -> analog) ===------ ///////
    switch(type)
    {
    case 0:
        ZP_LP2LP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5));
        break;
    case 1:
        ZP_LP2HP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5));
        break;
    case 2:
        ZP_LP2BP(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5), 2.0*tan(MY_PI*w2*0.5));
        break;
    case 3:
        ZP_LP2BS(Z, &nZ, P, &nP, &nZZ, &nZP, &gain, 2.0*tan(MY_PI*w1*0.5), 2.0*tan(MY_PI*w2*0.5));
        break;
    default:
        return -1;
        break;
    }
//////////////////////////////////////////////////////////////////////////////
/////// -----------==== transform: s-plain -> z-plain ===----------- /////////
    ZP_s2z(Z, &nZ, P, &nP, &gain, &nZZ, &nZP);
    *ga = gain.re;
    if (gain.im > deps)
    {
        //Ret = -1;
    }
    for (i = 0; i < nZ; i++)
    {
        ZRe[i] = Z[i].re;
        ZIm[i] = Z[i].im;
    }
    poly(Z, nZ+1);
    for (i = 0; i < nZ+1; i++)
    {
        Num[i] = Z[i].re*gain.re;
        if (Z[i].im > deps*abs(Z[i].re))
        {
            //Ret = -1;
        }
    }
    for (i = 0; i < nP; i++)
    {
        PRe[i] = P[i].re;
        PIm[i] = P[i].im;
    }
    poly(P, nP+1);
    for (i = 0; i < nP+1; i++)
    {
        Den[i] = P[i].re;
        if (P[i].im > deps*abs(P[i].re))
        {
            //Ret = -1;
        }
    }

    return Ret;
}