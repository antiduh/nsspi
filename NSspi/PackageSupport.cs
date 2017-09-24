using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NSspi
{
    /// <summary>
    /// Queries information about security packages.
    /// </summary>
    public static class PackageSupport
    {
        /// <summary>
        /// Returns the properties of the named package.
        /// </summary>
        /// <param name="packageName">The name of the package.</param>
        /// <returns></returns>
        public static SecPkgInfo GetPackageCapabilities( string packageName )
        {
            SecPkgInfo info;
            SecurityStatus status = SecurityStatus.InternalError;

            IntPtr rawInfoPtr;

            rawInfoPtr = new IntPtr();
            info = new SecPkgInfo();

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            { }
            finally
            {
                status = NativeMethods.QuerySecurityPackageInfo( packageName, ref rawInfoPtr );

                if( rawInfoPtr != IntPtr.Zero )
                {
                    try
                    {
                        if( status == SecurityStatus.OK )
                        {
                            // This performs allocations as it makes room for the strings contained in the SecPkgInfo class.
                            Marshal.PtrToStructure( rawInfoPtr, info );
                        }
                    }
                    finally
                    {
                        NativeMethods.FreeContextBuffer( rawInfoPtr );
                    }
                }
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to query security package provider details", status );
            }

            return info;
        }

        /// <summary>
        /// Returns a list of all known security package providers and their properties.
        /// </summary>
        /// <returns></returns>
        public static SecPkgInfo[] EnumeratePackages()
        {
            SecurityStatus status = SecurityStatus.InternalError;
            SecPkgInfo[] packages = null;
            IntPtr pkgArrayPtr;
            IntPtr pkgPtr;
            int numPackages = 0;
            int pkgSize = Marshal.SizeOf( typeof( SecPkgInfo ) );

            pkgArrayPtr = new IntPtr();

            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                status = NativeMethods.EnumerateSecurityPackages( ref numPackages, ref pkgArrayPtr );

                if( pkgArrayPtr != IntPtr.Zero )
                {
                    try
                    {
                        if( status == SecurityStatus.OK )
                        {
                            // Bwooop Bwooop Alocation Alert
                            // 1) We allocate the array
                            // 2) We allocate the individual elements in the array (they're class objects).
                            // 3) We allocate the strings in the individual elements in the array when we
                            //    call Marshal.PtrToStructure()

                            packages = new SecPkgInfo[numPackages];

                            for( int i = 0; i < numPackages; i++ )
                            {
                                packages[i] = new SecPkgInfo();
                            }

                            for( int i = 0; i < numPackages; i++ )
                            {
                                pkgPtr = IntPtr.Add( pkgArrayPtr, i * pkgSize );

                                Marshal.PtrToStructure( pkgPtr, packages[i] );
                            }
                        }
                    }
                    finally
                    {
                        NativeMethods.FreeContextBuffer( pkgArrayPtr );
                    }
                }
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to enumerate security package providers", status );
            }

            return packages;
        }
    }
}