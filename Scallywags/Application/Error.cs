using System;

namespace Scallywags
{
    /** @class  Error
     *  @brief  a static class to handle error output consistently
     */
    static class Error
    {
        //TODO...

        public static void Trace( string strMessage )
        {
#if WINDOWS
            Console.Out.WriteLine( strMessage );
#endif
        }
    }
}
