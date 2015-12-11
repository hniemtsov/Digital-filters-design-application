#include "matrix_comput.h"
#include "type_utils.h"
#include <vector>

// Design mse optimal filter based on tihonovs filtering theory using markov process approach
//this is function for filter non-overlap vector of size equal La
extern "C" __declspec(dllexport)
int noar( IS_CANCEL IsCancel,
			double *ar,
            int nAr,     // length of the impulse response, (FIR filter length)
            double std1, // symmetric or antysimmetric:  1 - anty; 0 - symmetric
            double std2,// cuttof frequency, always must be even
            double *in,// amplitude response in fr[i], length should be the same as fr
            double *out, // weights, length(g) = length(mg)/2 
            int n)
{
int i,j,k;
int M = nAr; 
std::vector<double> A(M*M);
std::vector<double> f(M);
std::vector<double> B(M*M);
std::vector<double> TMP(M*M);
std::vector<double> D1_BKBt(M*M);
std::vector<double> K(M*M);
std::vector<double> D1(M*M);
std::vector<double> D2(M*M);

double dev1 = std1*std1;
double dev2 = std2*std2;
double tmp1, tmp2;

for(i = 0; i < M; i++)
{
	A[i+i*M] = 1.0;
	D2[i+i*M]= dev2;
	for(j = i + 1; j < M; j++)
	{
		A[i*M+j] = ar[j-(i+1)];
		B[(M-1-j)*M+(M-1-i)] = -ar[M-2-(j-(i+1))];
	}
	B[(M-1-i)+(M-1-i)*M] = -ar[M-1];
}
invmatrixL2(&A[0], M);
for(i = M-1; i >=0; i--)
{
	for(j = M-1; j >= 0; j--)
	{
		tmp1 = 0;
		tmp2 = 0;
		int minij = i<j ? i : j;
		for(k = minij; k >= 0; k--)
		{
			tmp1 += A[k+i*M]*B[j+k*M];
			tmp2 += A[k+i*M]*A[k+j*M];
		}
		TMP[j+i*M]  = tmp1;
		D1[j+i*M]   = tmp2*dev1; // D1 = Ai*D1*Ai'
		K[j+i*M]    = D1[j+i*M];
	}
	K[i+i*M] = K[i+i*M] + dev2; // K = D1+D2
}
memcpy(B.data(), TMP.data(), M*M*sizeof(double));
CholeskyFact(&K[0], M); //(D1+D2)^-1
invmatrixL(&K[0], M);   //
for(i = 0; i < M; i++)
	for(j = 0; j < M; j++)
	{
		tmp1 = 0;
		for(k = 0; k < M; k++)
		{
			tmp1 += K[k+i*M]*D1[j+k*M];
		}
		TMP[j+i*M] = tmp1;
	}
if (n < nAr) return -1;
int idx = 0;
memcpy(K.data(), TMP.data(), M*M*sizeof(double)); // K = (D1+D2)^-1 * D1
for (i = 0; i < M; i++)
{
	out[idx] = 0;
	for(k = 0; k < nAr; k++) // M == nAr
	{
		out[idx] += TMP[k+i*M]*in[k];
		K[k+i*M] *= dev2; // K = K * D2
	}
	idx++;
}
for(; idx < n-nAr+1; idx+=nAr)
{
	for(i = 0; i < M; i++)
		for(j = 0; j < M; j++)
		{
			D2[j+i*M] = 0;
			for(k = 0; k < M; k++)
			{
				D2[j+i*M] += K[k+i*M]*B[k+j*M];
			}
		}
	for(i = 0; i < M; i++)
	{
		for(j = 0; j < M; j++)
		{
			tmp1 = 0;
			for(k = 0; k < M; k++)
			{
				tmp1 += B[k+i*M]*D2[j+k*M];
			}
			D1_BKBt[j+i*M] = tmp1 + D1[j+i*M]; // D2 is for temporary usege
			TMP[j+i*M] = D1_BKBt[j+i*M];
		}
		TMP[i+i*M] += dev2; // D1_BKBt + D2
	}
	CholeskyFact(TMP.data(), M); //(D1_BKBt+D2')^-1
    invmatrixL(TMP.data(), M);   //
	for (i = 0; i < M; i++)
	{
		f[i] = 0;
		for(k = 0; k < nAr; k++) // M == nAr
		{
			f[i] += dev2*B[k+i*M]*out[idx-nAr+k]; // D2 * B * filtered
		}
	}
	for (i = 0; i < M; i++)
	{
		out[idx+i] = 0;
		for(k = 0; k < nAr; k++) // M == nAr
		{
			out[idx+i] += TMP[k+i*M]*f[k]; // (D1_BKBt+D2)^-1 * D2 * B * filtered
		}
	}
	for (i = 0; i < M; i++)
	{
		f[i] = 0;
		for(k = 0; k < nAr; k++) // M == nAr
		{
			f[i] += D1_BKBt[k+i*M]*in[idx+k];//(D1_BKBt+D2)^-1 * input
		}
	}
	for (i = 0; i < M; i++)
		for(k = 0; k < nAr; k++) // M == nAr
		{
			out[idx+i] += TMP[k+i*M]*f[k];//(D1_BKBt+D2)^-1 * input
		}
////Complete compute the filtere vector
////Begin compute K
	for(i = 0; i < M; i++)
		for(j = 0; j < M; j++)
		{
			K[j+i*M] = 0;
			for(k = 0; k < M; k++)
			{
				K[j+i*M] += TMP[k+i*M]*D1_BKBt[j+k*M]*dev2;
			}
		}
}//

return 0;
}

extern "C" __declspec(dllexport)
int noarK( IS_CANCEL IsCancel,
			double *ar,
            int nAr,     // length of the impulse response, (FIR filter length)
            double std1, // symmetric or antysimmetric:  1 - anty; 0 - symmetric
            double std2,// cuttof frequency, always must be even
			double *Kout,
            int n)
{
int i,j,k,kk;
int M = nAr; 
std::vector<double> A(M*M);
std::vector<double> B(M*M);
std::vector<double> TMP(M*M);
std::vector<double> D1_BKBt(M*M);
std::vector<double> K(M*M);
std::vector<double> D1(M*M);
std::vector<double> D2(M*M);

double dev1 = std1*std1;
double dev2 = std2*std2;
double tmp1, tmp2;

for(i = 0; i < M; i++)
{
	A[i+i*M] = 1.0;
	D2[i+i*M]= dev2;
	for(j = i + 1; j < M; j++)
	{
		A[i*M+j] = ar[j-(i+1)];
		B[(M-1-j)*M+(M-1-i)] = -ar[M-2-(j-(i+1))];
	}
	B[(M-1-i)+(M-1-i)*M] = -ar[M-1];
}
invmatrixL2(&A[0], M);
for(i = M-1; i >=0; i--)
{
	for(j = M-1; j >= 0; j--)
	{
		tmp1 = 0;
		tmp2 = 0;
		int minij = i<j ? i : j;
		for(k = minij; k >= 0; k--)
		{
			tmp1 += A[k+i*M]*B[j+k*M];
			tmp2 += A[k+i*M]*A[k+j*M];
		}
		TMP[j+i*M]  = tmp1;
		D1[j+i*M]   = tmp2*dev1; // D1 = Ai*D1*Ai'
		K[j+i*M]    = D1[j+i*M];
	}
	K[i+i*M] = K[i+i*M] + dev2; // K = D1+D2
}
memcpy(B.data(), TMP.data(), M*M*sizeof(double));
CholeskyFact(&K[0], M); //(D1+D2)^-1
invmatrixL(&K[0], M);   //
for(i = 0; i < M; i++)
	for(j = 0; j < M; j++)
	{
		tmp1 = 0;
		for(k = 0; k < M; k++)
		{
			tmp1 += K[k+i*M]*D1[j+k*M];
		}
		TMP[j+i*M] = tmp1;
	}
if (n < nAr) return -1;
int idx = 0;
memcpy(K.data(), TMP.data(), M*M*sizeof(double)); // K = (D1+D2)^-1 * D1
for (i = 0; i < M; i++)
{
	for(k = 0; k < nAr; k++) // M == nAr
	{
		K[k+i*M] *= dev2; // K = K * D2
	}
	idx++;
}
kk=0;
Kout[kk] = K[0];
kk=kk+1;
for(; idx < n-nAr+1; idx+=nAr)
{
	for(i = 0; i < M; i++)
		for(j = 0; j < M; j++)
		{
			D2[j+i*M] = 0;
			for(k = 0; k < M; k++)
			{
				D2[j+i*M] += K[k+i*M]*B[k+j*M];
			}
		}
	for(i = 0; i < M; i++)
	{
		for(j = 0; j < M; j++)
		{
			tmp1 = 0;
			for(k = 0; k < M; k++)
			{
				tmp1 += B[k+i*M]*D2[j+k*M];
			}
			D1_BKBt[j+i*M] = tmp1 + D1[j+i*M];
			TMP[j+i*M] = D1_BKBt[j+i*M];
		}
		TMP[i+i*M] += dev2; // D1_BKBt + D2
	}
	CholeskyFact(TMP.data(), M); //(D1_BKBt+D2')^-1
    invmatrixL(TMP.data(), M);   //
	for(i = 0; i < M; i++)
		for(j = 0; j < M; j++)
		{
			TMP[j+i*M] *= dev2; // (D1_BKBt+D2)^-1 * D2
		}
////Complete compute the filtere vector
////Begin compute K
	for(i = 0; i < M; i++)
		for(j = 0; j < M; j++)
		{
			K[j+i*M] = 0;
			for(k = 0; k < M; k++)
			{
				K[j+i*M] += TMP[k+i*M]*D1_BKBt[j+k*M]*dev2;
			}
		}
    Kout[kk] = K[0];
	kk=kk+1;
}
return 0;
}
