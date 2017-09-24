using System;
using System.Reflection;

namespace NSspi
{
    [AttributeUsage( AttributeTargets.Field )]
    public class EnumStringAttribute : Attribute
    {
        public EnumStringAttribute( string text )
        {
            this.Text = text;
        }

        public string Text { get; private set; }
    }

    public class EnumMgr
    {
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