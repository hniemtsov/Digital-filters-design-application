// DFilter.cpp : Defines the exported functions for the DLL application.
//


//#include "ipp.h"
#include <stdlib.h>
#include <limits>
#include "math.h"
#include <time.h>
#include <memory.h>
#include "type_utils.h"
#include "ran.h"

extern "C" __declspec(dllexport)
int magnitude(IS_CANCEL IsCancel,
           double Num[],    // 
           double Den[],    //
           int LenNum,
           int LenDen,
           double A[],
           int nPoint)
{
    double ww;
    int i, j;
    double Pnir, Pnii;
    double Pdir, Pdii;

    int Ret = 0;

    for (i = 0; i < nPoint; i++)
    {
        Pnir = Num[0];
        Pnii = 0.0;
        for (j = 1; j < LenNum; j++)
        {
            ww = ((double)j)*((double)i)*MY_PI/((double)nPoint);
            Pnir += Num[j]*cos(ww);
            Pnii += Num[j]*sin(ww);
        }
        Pdir = Den[0];
        Pdii = 0.0;
        for (j = 1; j < LenDen; j++)
        {
            ww = ((double)j)*((double)i)*MY_PI/((double)nPoint);
            Pdir += Den[j]*cos(ww);
            Pdii += Den[j]*sin(ww);
        }
        A[i] = sqrt((Pnir*Pnir + Pnii*Pnii)/(Pdir*Pdir + Pdii*Pdii));
        if ( ((i % 32) == 0) 
            && (IsCancel()) )
        {
            Ret = -1;
            break;
        }
    }

    return Ret;
}

#define MAX_ORDER 100

int imprespDN(IS_CANCEL IsCancel,
            double Num[],    // 
            double Den[],    //
            int LenNum,
            int LenDen,
            double y[],
            int nPoint)
{
    int i, j;
    double xi, yi;
    int Ret = 0;
    double mem[MAX_ORDER];

    for(i = 0; i < LenNum-1; i++) mem[i] = 0.0;

    for (i = 0; i < nPoint; i++)
    {
        xi = (i == 0) ? 1.0 : 0.0;
        yi = xi*Num[0]/Den[0] + mem[0];
        for(j = 0; j < LenDen - 2; j++)
        {
            mem[j] = (mem[j+1] + Num[j+1]*xi/Den[0]) - Den[j+1]*yi/Den[0];
        }
        for(j = ((LenDen - 2)>0 ? (LenDen - 2) : 0); j < LenNum-2; j++)
        {
            mem[j] = mem[j+1] + Num[j+1]*xi/Den[0];
        }
        mem[LenNum-2] = Num[LenNum-1]*xi/Den[0];
        y[i] = yi;
        if ( ((i % 32) == 0) 
            && (IsCancel()) )
        {
            Ret = -1;
            break;
        }
    }

    return Ret;
}

int imprespND(IS_CANCEL IsCancel,
            double Num[],    // 
            double Den[],    //
            int LenNum,
            int LenDen,
            double y[],
            int nPoint)
{
    int i, j;
    double xi, yi;
    int Ret = 0;
    double mem[MAX_ORDER];

    for(i = 0; i < LenDen-1; i++) mem[i] = 0.0;

    for (i = 0; i < nPoint; i++)
    {
        xi = (i == 0) ? 1.0 : 0.0;
        yi = xi*Num[0]/Den[0] + mem[0];
        for(j = 0; j < LenNum - 2; j++)
        {
            mem[j] = (mem[j+1] + Num[j+1]*xi/Den[0]) - Den[j+1]*yi/Den[0];
        }
        for(j = LenNum - 2; j < LenDen-2; j++)
        {
            mem[j] = mem[j+1] - Den[j+1]*yi/Den[0];
        }
        mem[LenDen-2] = - Den[LenDen-1]*yi/Den[0];
        y[i] = yi;
        if ( ((i % 32) == 0) 
            && (IsCancel()) )
        {
            Ret = -1;
            break;
        }
    }

    return Ret;
}

int _impresp(IS_CANCEL IsCancel,
            double Num[],    // 
            double Den[],    //
            int LenNum,
            int LenDen,
            double y[],
            int nPoint)
{
    int i, j;
    double xi, yi;
    int Ret = 0;
    double mem[MAX_ORDER];

    for(i = 0; i < LenDen-1; i++) mem[i] = 0.0;

    for (i = 0; i < nPoint; i++)
    {
        xi = (i == 0) ? 1.0 : 0.0;
        yi = xi*Num[0]/Den[0] + mem[0];
        for(j = 0; j < LenNum - 2; j++)
        {
            mem[j] = (mem[j+1] + Num[j+1]*xi/Den[0]) - Den[j+1]*yi/Den[0];
        }
        mem[LenNum-2] = Num[LenNum-1]*xi/Den[0] - Den[LenDen-1]*yi/Den[0];
        y[i] = yi;
        if ( ((i % 32) == 0) 
            && (IsCancel()) )
        {
            Ret = -1;
            break;
        }
    }

    return Ret;
}
 
extern "C" __declspec(dllexport)
int impresp(IS_CANCEL IsCancel,
            double Num[],    // 
            double Den[],    //
            int LenNum,
            int LenDen,
            double y[],
            int nPoint)
{
    int Ret = 0;

    if (LenNum < LenDen)
    {
        Ret = imprespND(IsCancel, Num, Den, LenNum, LenDen, y, nPoint);
    }
    if (LenNum > LenDen)
    {
        Ret = imprespDN(IsCancel, Num, Den, LenNum, LenDen, y, nPoint);
    }
    if (LenNum == LenDen)
    {
        Ret = _impresp(IsCancel, Num, Den, LenNum, LenDen, y, nPoint);
    }

    return Ret;
}


extern "C" __declspec(dllexport)
int
firfs(IS_CANCEL IsCancel,
      int  len,         // filter length
      double *fs, // band edges 
      double *A,  // amplitudes (length(A) == length(fs))
      int nA,           // even length of array A
      int nFFT,         // number of FFT points
      int symet,        // 0 - symmetric 
      double *coef)     // result output (filter coefficients)
{
    /*
    int i, j;
    int type;
    double dx, x;
    double d_omega;
    double si = 1.0;
    double *omega;
    double *PP;
    int N, N2; //length signal for FFT

    int orderFFT;
    
    Ipp64f *pSrcDstRe, *pSrcDstIm;
    IppsFFTSpec_C_64f* ppFFTSpec;
    IppHintAlgorithm hint = ippAlgHintAccurate;
    int flag = IPP_FFT_DIV_INV_BY_N;//IPP_FFT_NODIV_BY_ANY
    int pBufferSize;
    Ipp8u* pBuffer;

    if (1==(nA & 0x01)) return -1;
    if ((fs[0] != 0.0) || (fs[nA-1] != 1.0)) return -1;
    if (A[0] < 0) return -1;
    for (i = 1; i < nA; i++)
    {
        if ((fs[i] - fs[i-1]) < 0) return -1;
        if (A[i] < 0) return -1;
    }

    type = (symet << 1) + (!(len & 0x01)); // 0-symetric and odd length impulse response;      
                                           // 1-symetric and even length impulse response;     pi
                                           // 2-antisymetric and odd length impulse response;  0, pi
                                           // 3-antisymetric and even length impulse response; 0
    if ((1 == type) && (A[nA-1] != 0)) return -1;
    if ((2 == type) && ((A[nA-1] != 0) || (A[0] != 0))) return -1;
    if ((3 == type) && (A[0] != 0)) return -1;

    if ((1 == type) || (2 == type)) si = -1.0;

    for (orderFFT = 1; nFFT > (1<<orderFFT); orderFFT++);
    N = 1<<orderFFT;
    N2 = N << 1; 
    orderFFT++;
    if (N2 < len) return -1;
    dx = 1.0/(double)N;

    PP = (double*)malloc(N2*sizeof(double));
    pSrcDstRe = ippsMalloc_64f(N2);
    pSrcDstIm = ippsMalloc_64f(N2);
    omega = (double*)malloc(N2*sizeof(double));

    j = 0;
    PP[0] = A[0];
    for (i = 1; i < N; i++)
    {
        x = ((double)i)*dx;
        if (x > fs[j+1]) j += 1;
        PP[i] = A[j]*((fs[j+1]- x)/(fs[j+1]- fs[j])) + A[j+1]*((x - fs[j])/(fs[j+1]- fs[j]));
        PP[N2-i] = si*PP[i];
    }
    PP[N] = A[nA-1];

    d_omega = IPP_2PI/(double)N2;
    for (i = 0; i < N2-1; i++)
    {
        omega[i] = i * d_omega * 0.5 * (1.-(double)len);
    }
    omega[N2-1] = IPP_2PI * ((double)N2-1.) * 0.5 * (1.-(double)len) / (double)N2 ;
    if ((2 == type) || (3 == type))
    {
        for (i = 0; i < N2; i++) omega[i] += IPP_PI*0.5;
    }
    for (i = 0; i < N2; i++)
    {
        pSrcDstIm[i] = (Ipp64f)(sin(omega[i])*PP[i]);
        pSrcDstRe[i] = (Ipp64f)(cos(omega[i])*PP[i]);
    }
    
    ippsFFTInitAlloc_C_64f(&ppFFTSpec, orderFFT, flag, hint);
    ippsFFTGetBufSize_C_64f(ppFFTSpec, &pBufferSize);
    pBuffer = ippsMalloc_8u(pBufferSize);
    ippsFFTInv_CToC_64f_I(pSrcDstRe, pSrcDstIm, ppFFTSpec, pBuffer);
    for (i = 0; i < len; i++)
	{
        coef[i] = (double)pSrcDstRe[i];
    }

    ippsFree(pBuffer);
    ippsFree(pSrcDstRe);
    ippsFree(pSrcDstIm);
    free(omega);
    free(PP);
    */
    return 0;
}

extern "C" __declspec(dllexport)
int
bpsk_model(IS_CANCEL IsCancel,
         int N,
         double fc,
         double fs,
         double br,
         double *bs,
         int nBs,
         double *x)
{
    double cur_time = 0.0;
    double dt = 1.0/(fs*1000.0);
    double dT = 1.0/(br*1000.0);
    double T = dT;
    double bit;
    int i, j;
    j = 0;

    bit = bs[j];
    for(i = 0; i < N; i++)
    {
        x[i] = bit*sin(2*MY_PI*(fc*1000.0)*cur_time);
        if (cur_time > T)
        {
            T += dT;
            j++;
            if (j >= nBs) j = 0;
            bit = bs[j];
        }
        cur_time += dt;
    }
    return 0;
}

extern "C" __declspec(dllexport)
int
wn_model(IS_CANCEL IsCancel,
         int N,
         double std,
         double mean,
         double *x)
{
    
    
    
    time_t rawtime;
    struct tm timeinfo;
    time ( &rawtime );
    localtime_s(&timeinfo, &rawtime);

    Normaldev_BM gau(mean,std,(unsigned long long int)(timeinfo.tm_sec));
    for(int i = 0; i < N; i++)
    {
        x[i] = gau.dev();
    }
    /*
    Ipp32f in;
    std::
    IppsRandGaussState_32f *pRandGaussState;
    if (ippsRandGaussInitAlloc_32f(&pRandGaussState, 0, (Ipp32f) std, timeinfo.tm_sec) != ippStsNoErr)
    {
        return -1;
    }

    for(int i = 0; i < N; i++)
    {
        if (ippsRandGauss_32f(&in, 1, pRandGaussState) != ippStsNoErr)
        {
            ippsRandGaussFree_32f(pRandGaussState);
            return -1;
        }
        x[i] = (double)in;
    }
    ippsRandGaussFree_32f(pRandGaussState);
    */
    return 0;
}


extern "C" __declspec(dllexport)
int
ar_model(IS_CANCEL IsCancel,
         int N,
         double std,
         double *arc,
         double *mem,
         int narc,
         double *x)
{
    
    
    time_t rawtime;
    struct tm timeinfo;
    time ( &rawtime );
    localtime_s(&timeinfo, &rawtime);

    Normaldev_BM gau(0.0,std,(unsigned long long int)(timeinfo.tm_sec));
    for(int i = 0; i < N; i++)
    {
        double in = gau.dev();
        x[i] = in/arc[0] + mem[0];
        for (int j = 0; j < narc-2; j++)
        {
            mem[j] = mem[j+1] - arc[j+1]*x[i]/arc[0];
        }
        mem[narc-2] = -x[i]*arc[narc-1]/arc[0]; 
    }

    /*
    Ipp32f in;
    IppsRandGaussState_32f *pRandGaussState;
    if (ippsRandGaussInitAlloc_32f(&pRandGaussState, 0, (Ipp32f) std, timeinfo.tm_sec) != ippStsNoErr)
    {
        return -1;
    }

    for(int i = 0; i < N; i++)
    {
        if (ippsRandGauss_32f(&in, 1, pRandGaussState) != ippStsNoErr)
        {
            ippsRandGaussFree_32f(pRandGaussState);
            return -1;
        }
        x[i] = ((double)in)/arc[0] + mem[0];
        for (int j = 0; j < narc-2; j++)
        {
            mem[j] = mem[j+1] - arc[j+1]*x[i]/arc[0];
        }
        mem[narc-2] = -x[i]*arc[narc-1]/arc[0]; 
    }
    ippsRandGaussFree_32f(pRandGaussState);
    */
    return 0;
}

void filter_ab(double *b, int nb, double *a, int na, double *x, double *y, int n)
{
    double mem[MAX_ORDER];
    double xi, yi;
    int i, j;
    memset(mem, 0, nb*sizeof(double));

    for (i = 0; i < n; i++)
    {
        xi = x[i];
        yi = xi*b[0]/a[0] + mem[0];
        for(j = 0; j < na - 2; j++)
        {
            mem[j] = mem[j+1] + b[j+1]*xi/a[0] - a[j+1]*yi/a[0];
        }
        for(j = na-2; j < nb-2; j++)
        {
            mem[j] = mem[j+1] + b[j+1]*xi/a[0];
        }
        mem[nb-2] = b[nb-1]*xi/a[0];
        y[i] = yi;
    }

    return;
}

void filter_ba(double *b, int nb, double *a, int na, double *x, double *y, int n)
{
    double mem[MAX_ORDER];
    double xi, yi;
    int i, j;
    memset(mem, 0, na*sizeof(double));

    for (i = 0; i < n; i++)
    {
        xi = x[i];
        yi = xi*b[0]/a[0] + mem[0];
        for(j = 0; j < nb - 2; j++)
        {
            mem[j] = mem[j+1] + b[j+1]*xi/a[0] - a[j+1]*yi/a[0];
        }
        for(j = nb-2; j < na-2; j++)
        {
            mem[j] = mem[j+1] - a[j+1]*yi/a[0];
        }
        mem[nb-2] = -a[na-1]*yi/a[0];
        y[i] = yi;
    }

    return;
}

void firfilter(double *b, int nb, double *x, double *y, int n)
{
    double mem[MAX_ORDER];
    double xi, yi;
    int i, j;
    memset(mem, 0, nb*sizeof(double));

    for (i = 0; i < n; i++)
    {
        xi = x[i];
        yi = xi*b[0] + mem[0];
        for(j = 0; j < nb - 2; j++)
        {
            mem[j] = mem[j+1] + b[j+1]*xi;
        }
        mem[nb-2] = b[nb-1]*xi;
        y[i] = yi;
    }

    return;
}

void iirfilter(double *a, int na, double *x, double *y, int n)
{
    double mem[MAX_ORDER];
    double yi;
    int j, i;
    memset(mem, 0, na*sizeof(double));

    for (i = 0; i < n; i++)
    {
        yi = x[i]/a[0] + mem[0];
        for(j = 0; j < na - 2; j++)
        {
            mem[j] = mem[j+1] - a[j+1]*yi/a[0];
        }
        mem[na-2] = -a[na-1]*yi/a[0];
        y[i] = yi;
    }

    return;
}

void filter(double *b, double *a, int n, double *x, double *y, int ny)
{
    double mem[MAX_ORDER];
    double xi, yi;
    int i, j;
    memset(mem, 0, n*sizeof(double));

    for (i = 0; i < ny; i++)
    {
        xi = x[i];
        yi = xi*b[0]/a[0] + mem[0];
        for(j = 0; j < n - 2; j++)
        {
            mem[j] = mem[j+1] + b[j+1]*xi/a[0] - a[j+1]*yi/a[0];
        }
        mem[n-2] = b[n-1]*xi/a[0] - a[n-1]*yi/a[0];
        y[i] = yi;
    }

    return;
}

extern "C" __declspec(dllexport)
int
do_filter(IS_CANCEL IsCancel,
        double *b,
        double *a,
        int nb,
        int na,
        double *x,
        double *y,
        int n)
{
    if (nb < na)
    {
        if (nb == 1)
        {
            iirfilter(a, na, x, y, n);
        }
        else
        {
            filter_ba(b, nb, a, na, x, y, n);
        }
    }
    else
    {
        if (na == nb)
        {
            filter(b, a, na, x, y, n);
        }
        else if (na == 1)
        {
            iirfilter(b, nb, x, y, n);
        }
        else
        {
            filter_ab(b, nb, a, na, x, y, n);
        }
    }
    return 0;
}