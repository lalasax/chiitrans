using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiiTrans
{
    public class Djon
    {
	    private static int[] Z_function( string s )
	    {
    		int n = s.Length;
		    int i, l = 0, r = 0;
            int[] z = new int[n];
    		z[0] = 0;
	    	for ( i = 1; i < n; ++ i )
		    {
			    if ( i < r )
			    {
				    if ( z[i - l] >= r - i )
				    {
    					l = i;
	    				while ( r < n && s[r] == s[r - l] )
		    				++ r;
			    		z[i] = r - l;
				    }
				    else 
                    {
					    z[i] = z[i - l];
                    }
    			}
	    		else
		    	{
			    	r = i;
    				l = i;
	    			while ( r < n && s[r] == s[r - l] )
		    			++ r;
			    	z[i] = r - l;
			    }
		    }
            return z;
        }

	public static List<string> GetRepeatingPhrases( string s, int maxn, out int n )
	{
		int i, j, k, clen;
		int len = s.Length;
		for ( i = maxn; i >= 1; -- i )
		{
			if ( ( len % i ) == 0 )
				break;
		}
		int biggest_n = i;
        int[,] last = new int[len + 1, biggest_n];
        int[] d = new int[len + 1];
		d[0] = ( 1 << ( biggest_n + 1 ) ) - 1; // mask of possible dividing
		for ( i = 0; i < len; ++ i ) // start position
		{
			if ( d[i] > 0 )
			{
				int[] z = Z_function( s.Substring( i ) );
				for ( clen = 1; i + clen <= len; ++ clen ) // current len
				{
					for ( k = 1; k <= biggest_n; ++ k )
					{
						if ( (d[i] & ( 1 << k )) != 0 )
						{
							d[i + k * clen] |= 1 << k;
							last[i + k * clen, k - 1] = i;
						}
						if ( !( i + k * clen < len && z[k * clen] >= clen ) ) //length of prefix(i+k*clen) equal to prefix(0) is more than clen
							break;							
					}
				}
			}
		}
		for ( i = biggest_n; i >= 1; -- i )
		{
			if ( (d[len] & ( 1 << i )) != 0 )
			{
				break;
			}
		}
        if (i < 1)
            throw new Exception();
		n = i;
		List<string> ans = new List<string>(); 
		for ( i = len; i > 0; i = j )
		{
			j = last[i, n - 1];
            ans.Add(s.Substring(j, ( i - j ) / n ));
		}
        ans.Reverse();
		return ans;
	}

    }
}
