#include "matrix_comput.h"
#include "type_utils.h"
#include <vector>

typedef enum  _Symmetry
{
    even = 0,
    odd   = 1
}  Symmetry; 

// Design FIR filter based on the least square error criterion
extern "C" __declspec(dllexport)
int firls( IS_CANCEL IsCancel,
			double *tap,
            int N,     // length of the impulse response, (FIR filter length)
            int type, // symmetric or antysimmetric:  1 - anty; 0 - symmetric
            double *f,// cuttof frequency, always must be even
            double *mg,// amplitude response in fr[i], length should be the same as fr
            double *g, // weights, length(g) = length(mg)/2 
            int len_g)
{
    int M = (N>>1);
    int typeI = (type == even)*(N & 0x01);
    M += typeI;
    std::vector<double> Q(M*M); // Upper triangular matrix
    std::vector<double> b(M); // right part of the eq. : Qx = b
    double evenN = (N & 0x01) == 0;
    int i, i2;
    double A, B;
    int k;
    int n;
	std::vector<double> fr(len_g*2);

    for (i = 0; i < len_g<<1; i++)
    {
        fr[i] = f[i]*MY_PI;
    }

    if (typeI)
    {   //1-st row
        int k = 0;
        int n = 1;
        for (n = k+1; n < M; n++)
        {
            for(i = 0; i < len_g; i++)
            {
                i2 = i<<1;
                Q[n + k*M] += 0.5*g[i]*(sin((n-k)*fr[i2+1]) - sin((n-k)*fr[i2]))/(n-k);
                Q[n + k*M] += 0.5*g[i]*(sin((n+k)*fr[i2+1]) - sin((n+k)*fr[i2]))/(n+k);
            }
            Q[k + n*M] = Q[n + k*M];
        }
        for(i = 0; i < len_g; i++)
        {
            i2 = i<<1;
            Q[k + k*M] += g[i]*(fr[i2+1] - fr[i2]);
            A = (mg[i2+1]-mg[i2])/(fr[i2+1]-fr[i2]);
            B = (fr[i2+1]*mg[i2] - fr[i2]*mg[i2+1])/(fr[i2+1]-fr[i2]);
            b[k] += A*g[i]*(fr[i2+1]*fr[i2+1] - fr[i2]*fr[i2])*0.5;
            b[k] += B*g[i]*(fr[i2+1] - fr[i2]);
        }
        // end 1-st and start othes rows
        for (k = 1; k < M; k++)
        {
            for (n = k+1; n < M; n++)
            {
                for(i = 0; i < len_g; i++)
                {
                    i2 = i<<1;
                    Q[n + k*M] += 0.5*g[i]*(sin((n-k)*fr[i2+1]) - sin((n-k)*fr[i2]))/(n-k);
                    Q[n + k*M] += 0.5*g[i]*(sin((n+k)*fr[i2+1]) - sin((n+k)*fr[i2]))/(n+k);
                }
                Q[k + n*M] = Q[n + k*M];
            }
            for(i = 0; i < len_g; i++)
            {
                i2 = i<<1;
                Q[k + k*M] += 0.5*g[i]*(fr[i2+1] - fr[i2]);
                Q[k + k*M] += 0.5*g[i]*(sin(2*k*fr[i2+1]) - sin(2*k*fr[i2]))/(2*k);
                A = (mg[i2+1]-mg[i2])/(fr[i2+1]-fr[i2]);
                B = (fr[i2+1]*mg[i2] - fr[i2]*mg[i2+1])/(fr[i2+1]-fr[i2]);
                b[k] += A*g[i]*(fr[i2+1]*sin(k*fr[i2+1]) - fr[i2]*sin(k*fr[i2]))/k;
                b[k] += A*g[i]*(cos(k*fr[i2+1]) - cos(k*fr[i2]))/k/k;
                b[k] += B*g[i]*(sin(k*fr[i2+1]) - sin(k*fr[i2]))/k;
            }
        }
    }
    else //type II, III, IV
    {
        double antyPI = type*MY_PI;
        double antyPI2 = type*MY_PI2;

        for (int kk = 0; kk < M; kk++)
        {
            k = kk + 1;
            for (int nn = kk+1; nn < M; nn++)
            {
                n = nn + 1;
                for(i = 0; i < len_g; i++)
                {
                    i2 = i<<1;
                    Q[nn + kk*M] += 0.5*g[i]*(sin((n-k)*fr[i2+1]) - sin((n-k)*fr[i2]))/(n-k);
                    Q[nn + kk*M] += 0.5*g[i]*(sin((n+k-evenN)*fr[i2+1]-antyPI) - sin((n+k-evenN)*fr[i2]-antyPI))/(n+k-evenN);
                }
                Q[kk + nn*M] = Q[nn + kk*M];
            }
            for(i = 0; i < len_g; i++)
            {
                i2 = i<<1;
                Q[kk + kk*M] += 0.5*g[i]*(fr[i2+1] - fr[i2]);
                Q[kk + kk*M] += 0.5*g[i]*(sin((2*k-evenN)*fr[i2+1]-antyPI) - sin((2*k-evenN)*fr[i2]-antyPI))/(2*k-evenN);
                A = (mg[i2+1]-mg[i2])/(fr[i2+1]-fr[i2]);
                B = (fr[i2+1]*mg[i2] - fr[i2]*mg[i2+1])/(fr[i2+1]-fr[i2]);
                b[kk] += A*g[i]*(fr[i2+1]*sin((k-evenN*0.5)*fr[i2+1]-antyPI2) - fr[i2]*sin((k-evenN*0.5)*fr[i2]-antyPI2))/(k-evenN*0.5);
                b[kk] += A*g[i]*(cos((k-evenN*0.5)*fr[i2+1]-antyPI2) - cos((k-evenN*0.5)*fr[i2]-antyPI2))/(k-evenN*0.5)/(k-evenN*0.5);
                b[kk] += B*g[i]*(sin((k-evenN*0.5)*fr[i2+1]-antyPI2) - sin((k-evenN*0.5)*fr[i2]-antyPI2))/(k-evenN*0.5);
            }
        }
    }

    CholeskyFact(&Q[0], M);
    invmatrixL(&Q[0], M);
    memset(tap, 0, sizeof(double)*N);
    double *pTmp = &tap[N-M];
    double mul = (type == odd) ? -1. : 1.;
    int offs = (typeI !=1 ) + (evenN==0)*(typeI!=1);
    for (i = 0; i < M; i++)
    {
        for (k = 0; k < M; k++)
        {
            *pTmp += Q[k + i*M]*b[k]*0.5;
        }
       tap[N-M-i - offs] = (*pTmp)*mul;
        pTmp++;
    }
	if (N & 0x01) tap[N>>1] *= 2.0;
    return 0;
}