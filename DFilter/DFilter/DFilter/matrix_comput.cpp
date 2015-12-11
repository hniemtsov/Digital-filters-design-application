#include <vector>

int CholeskyFact(double *A, int n)
{
    std::vector<double> vV(n);
    double *v = &vV[0];
    std::vector<double> vG(n*n);
    double *G = &vG[0];

    for (int j = 0 ; j < n; j++)
    {
        memcpy_s(&v[j], (n-j)*sizeof(double), &A[j+j*n], (n-j)*sizeof(double));
        for (int k = 0; k < j; k++) 
        {
            for (int i = j; i < n; i++)
            {
                v[i] = v[i] - G[j + k*n]*G[i + k*n];
            }
        }
        memcpy_s(&G[j+j*n], (n-j)*sizeof(double), &v[j], (n-j)*sizeof(double));
        for (int i = j; i < n; i++)
        {
            G[i+j*n] /= sqrt(v[j]);
        }
    }
    memcpy_s(A, n*n*sizeof(double), G, n*n*sizeof(double));
	
	return 0;
}

int invmatrixU(double *A,
                int n) // [n X n]
{
    for (int k = 0; k < n; k++)
    {
        A[k + k * n] = 1/A[k + k * n];
        for (int rows = 0; rows < k; rows++)
        {
            A[k + rows * n] = A[k + rows * n] * A[k + k * n];
        }
        for (int col = k+1; col < n; col++)
        {
            for (int row = 0; row < k+1; row++)
            {
                A[col + row * n] = A[col + row * n] - A[k + col * n] * A[k + row * n];
            }
        }
    }
	return 0;
}
int invmatrixL(double *A,
                int n) // [n X n]
{
    for (int k = n-1; k >= 0; k--)
    {
        A[k + k * n] = 1/A[k + k * n];
        for (int rows = k+1; rows < n; rows++)
        {
            A[k + rows * n] = A[k + rows * n] * A[k + k * n];
        }
        //for (int col = k+1; col < n; col++)
        for (int col = 0; col < k; col++)
        {
            //for (int row = 0; row < k+1; row++)
            for (int row = k; row < n; row++)
            {
                A[col + row * n] = A[col + row * n] - A[k + col * n] * A[k + row * n];
            }
        }
    }
    for (int row = 0; row < n; row++)
    {
        for (int col = n-1; col >= row;  col--)
        {
            double tmp = 0;
            for (int k = n-1; k >= col; k--) 
            {
                tmp += A[col+k*n] * A[row+k*n];
            }
            A[col + row*n] = tmp;
        }
    }
    for (int row = 0; row < n; row++)
    {
        for (int col = n-1; col > row;  col--)
        {
            A[row + col*n] = A[col + row*n];
        }
    }
	return 0;
}
int invmatrixL2(double *A,
                int n) // [n X n]
{
    for (int k = n-1; k >= 0; k--)
    {
        A[k + k * n] = 1/A[k + k * n];
        for (int rows = k+1; rows < n; rows++)
        {
            A[k + rows * n] = A[k + rows * n] * A[k + k * n];
        }
        //for (int col = k+1; col < n; col++)
        for (int col = 0; col < k; col++)
        {
            //for (int row = 0; row < k+1; row++)
            for (int row = k; row < n; row++)
            {
                A[col + row * n] = A[col + row * n] - A[k + col * n] * A[k + row * n];
            }
        }
    }
	return 0;
}