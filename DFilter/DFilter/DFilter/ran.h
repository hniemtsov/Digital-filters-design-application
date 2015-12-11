#pragma once

#include <math.h>

struct Ran {
    unsigned long long int  u,v,w;
    Ran(unsigned long long int j) : v(4101842887655102017LL), w(1)
    {
        u = j ^ v; int64();
        v = u; int64();
        w = v; int64();
    }
    inline unsigned long long int int64()
    {
        u = u * 2862933555777941757LL + 7046029254386353087LL;
        v ^= v >> 17; v ^= v << 31; v ^= v >> 8;
        w = 4294957665U*(w & 0xffffffff) + (w >> 32);
        unsigned long long int x = u ^ (u << 21); x ^= x >> 35; x ^= x << 4;
        return (x + v) ^ w;
    }
    inline double doub() { return 5.42101086242752217E-20 * int64(); }
    inline unsigned int int32() { return (unsigned int)int64(); }
};

struct Normaldev_BM : Ran {
    double mu,sig;
    double storedval;
    Normaldev_BM(double mmu, double ssig, unsigned long long int i)
    : Ran(i), mu(mmu), sig(ssig), storedval(0.) {}
    double dev()
    {
        double v1,v2,rsq,fac;
        if (storedval == 0.) // We don’t have an extra deviate handy, so
        {  
            do
            {
                v1 = 2.0*doub() - 1.0; // pick two uniform numbers in the square 
                v2 = 2.0*doub() - 1.0; // extending from -1 to +1 in each direction,
                rsq = v1*v1 + v2*v2;   // see if they are in the unit circle,
            } while (rsq >= 1.0 || rsq == 0.0); // or try again.
            fac = sqrt(-2.0*log(rsq)/rsq); // Now make the Box-Muller transformation to
            storedval = v1*fac;            //get two normal deviates. Return one and
            return mu + sig*v2*fac;        // save the other for next time.
        } 
        else //We have an extra deviate handy,
        { 
            fac = storedval;
            storedval = 0.;
            return mu + sig*fac; //so return it.
        }
    }
};