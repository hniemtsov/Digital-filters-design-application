#pragma once

#include <stdlib.h>
#include <limits>
#include "math.h"
#include <time.h>
#include <memory.h>
#include "type_utils.h"
#include <vector>
#include <list>
#include <algorithm>
#include <set>

class Cmplx
{
public:
    Cmplx(double _Re, double _Im) : re(_Re), im(_Im)
    {
        abs = _Re*_Re + _Im*_Im;
    }

    class lt_abs
    {
    public:
        bool operator()(const Cmplx& x, const Cmplx& y)
        {
            return ((x.abs) < (y.abs));
        }
    };
    class gt_abs
    {
    public:
        bool operator()(const Cmplx& x, const Cmplx& y)
        {
            return (x.abs) < (y.abs);
        }
    };
    double abs;
    double re; // real value
    double im; // imaginary part
};


int zp2sos(IS_CANCEL IsCancel,
     int nZ,
     double *ZRe,
     double *ZIm,
     int nP,
     double* PRe,
     double* PIm,
     int sos_order,
     int sos_scale,
     double *gain,
     double *zk,
     int *nzk,
     double *pk,
     int *npk)
{
    int i;
    if (sos_order < 0) return -1;
    if (sos_scale < 0) return -1;

    std::vector<Cmplx> zm;
    std::vector<Cmplx> pm;

    for(i = 0; i < nZ; i++)
    {
        zm.push_back(Cmplx(ZRe[i], ZIm[i]));
    }
    for(i = 0; i < nP; i++)
    {
        pm.push_back(Cmplx(PRe[i], PIm[i]));
    }
    
    std::sort(zm.begin(), zm.end(), Cmplx::gt_abs());
    
    if (sos_order == 0)
    {
        std::sort(pm.begin(), pm.end(), Cmplx::gt_abs());
    }
    
    if (sos_order == 1)
    {
		std::sort(pm.begin(), pm.end(), Cmplx::lt_abs());
    }
    
    *npk = *nzk = 0;
    int minzJ = -1;
    int minpJ = -1;
    int minzK = -1;
    double mmin = 10.0;
    for(i = 0; i < pm.size(); i = 0)
    {
       mmin = 10.0;
       for(int j = i+1; j < pm.size(); j++)
       {
           if ((abs(pm[j].im+pm[i].im) < mmin) && (pm[j].im*pm[i].im < 0))
           {
               minpJ = j;
               mmin = abs(pm[j].im+pm[i].im);
           }
       }
       if (zm.size()>0)
       {
           minzJ = -1;
           mmin = 100;
           for(int j = 0; j < zm.size(); j++)
           {
               if (abs(zm[j].abs - pm[i].abs) < mmin)
               {
                   minzJ = j;
                   mmin = abs(zm[j].abs - pm[i].abs);
               }
           }
           mmin = 10;
           minzK = -1;
           for(int j = 0; j < zm.size(); j++)
           {
               if ((j != minzJ) && (abs(zm[j].im+zm[i].im) < mmin) && (zm[j].im*zm[i].im < 0))
               {
                   minzK = j;
                   mmin = abs(zm[j].im+zm[i].im);
               }
           }
           if (minzK == -1)
           {
            for(int j = 0; j < zm.size(); j++)
            {
               if ((j != minzJ) && (abs(zm[j].im+zm[i].im) < mmin) && (zm[j].im == 0))
               {
                   minzK = j;
                   mmin = abs(zm[j].im+zm[i].im);
               }
            }
           }
           if ((minzK == -1) && (zm.size() == 2))
           {
               minzK = 1;
           }
           if (minzK == -1)
           {
            zk[*nzk] = 1.0;
            (*nzk)++;
            zk[*nzk] = -(zm[minzJ].re);
            (*nzk)++;
            zk[*nzk] = 0;
            (*nzk)++;

            zm.erase(zm.begin()+minzJ, zm.begin()+minzJ+1);
           }
           else
           {
            zk[*nzk] = 1.0;
            (*nzk)++;
            zk[*nzk] = -(zm[minzJ].re+zm[minzK].re);
            (*nzk)++;
            zk[*nzk] = (zm[minzJ].re*zm[minzK].re-zm[minzJ].im*zm[minzK].im);
            (*nzk)++;
            zm.erase(zm.begin()+minzJ, zm.begin()+minzJ+1);
            if (minzJ < minzK)
            {
                minzK--;
            }
            zm.erase(zm.begin()+minzK, zm.begin()+minzK+1);
           }
       }
       else
       {
        zk[*nzk] = 1.0;
        (*nzk)++;
        zk[*nzk] = 0.0;
        (*nzk)++;
        zk[*nzk] = 0.0;
        (*nzk)++;
       }

       if (minpJ != -1)
       {
        pk[*npk] = 1.0;
        (*npk)++;
        pk[*npk] = -(pm[i].re+pm[minpJ].re);
        (*npk)++;
        pk[*npk] = (pm[minpJ].re*pm[i].re-pm[i].im*pm[minpJ].im);
        (*npk)++;
        
        pm.erase(pm.begin()+i, pm.begin()+i+1);
        minpJ--;
        pm.erase(pm.begin()+minpJ, pm.begin()+minpJ+1);
       }
       else
       {
        pk[*npk] = 1.0;
        (*npk)++;
        pk[*npk] = -(pm[i].re);
        (*npk)++;
        pk[*npk] = 0;
        (*npk)++;

        pm.erase(pm.begin()+i, pm.begin()+i+1);
       }
    }
    if (zm.size()>0) return -1;


    return 0;
}