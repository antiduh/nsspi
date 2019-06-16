using System;
using System.Reflection;

namespace NSspi
{
    /// <summary>
    /// Tags an enumeration member with a string that can be programmatically accessed.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field )]
    public class EnumStringAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStringAttribute"/> class.
        /// </summary>
        /// <param name="text">The string to associate with the enumeration member.</param>
        public EnumStringAttribute( string text )
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the string associated with the enumeration member.
        /// </summary>
        public string Text { get; private set; }
    }

    /// <summary>
    /// Converts betwen enumeration members and the strings associated to the members through the
    /// <see cref="EnumStringAttribute"/> type.
    /// </summary>
    public class EnumMgr
    {
        /// <summary>
        /// Gets the text associated with the given enumeration member through a <see cref="EnumStringAttribute"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToText( Enum value )
        {
            FieldInfo field = value.GetType().GetField( value.ToString() );

            EnumStringAttribute[] attribs = (EnumStringAttribute[])field.GetCustomAttributes( typeof( EnumStringAttribute ), false );

            if( attribs == null || attribs.Length == 0 )
            {
                return null;
            }
            else
            {
                return attribs[0].Text;
            }
        }

        /// <summary>
        /// Returns the enumeration member that is tagged with the given text using the <see cref="EnumStringAttribute"/> type.
        /// </summary>
        /// <typeparam name="T">The enumeration type to inspect.</typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T FromText<T>( string text )
        {
            FieldInfo[] fields = typeof( T ).GetFields();

            EnumStringAttribute[] attribs;

            foreach( FieldInfo field in fields )
            {
                attribs = (EnumStringAttribute[])field.GetCustomAttributes( typeof( EnumStringAttribute ), false );

                foreach( EnumStringAttribute attrib in attribs )
                {
                    if( attrib.Text == text )
                    {
                        return (T)field.GetValue( null );
                    }
                }
            }

            throw new ArgumentException( "Could not find a matching enumeration value for the text '" + text + "'." );
        }
    }
}