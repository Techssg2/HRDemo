using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using System.Reflection;

namespace Aeon.HR.UI
{
    public static class WebUtility
    {
        public static T GetQueryString<T>(this Page page, string key)
        {
            return WebUtility.GetQueryString<T>(page.Request.QueryString, key);
        }
        public static T GetQueryString<T>(this NameValueCollection queryString, string key)
        {
            T m_Data = default(T);

            try
            {
                string m_Value = queryString[key];
                if (m_Value == null)
                    m_Data = default(T);
                else if (typeof(T).Equals(typeof(string)))
                    m_Data = (T)(object)m_Value;
                else
                {
                    object[] m_TryParseArgs = new object[] { m_Value, default(T) };
                    var m_MethodInfo = (typeof(T)).GetMethod("TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType() });
                    if ((bool)m_MethodInfo.Invoke(null, m_TryParseArgs))
                        m_Data = (T)m_TryParseArgs[1];
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage("Aeon.ManagementPortalUI.WebUtility.GetQueryString", ex.ToString());
            }

            return m_Data;
        }
        public static string StripTagsCharArray(this string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }

            string m_TextReturn = string.Empty;

            if (arrayIndex > 400)
                m_TextReturn = new string(array, 0, 400) + "...";
            else
                m_TextReturn = new string(array, 0, arrayIndex);

            return m_TextReturn;
        }

        public static void SelectDropDownListItemByText(this DropDownList dropDownList, string text)
        {
            dropDownList.SelectedIndex = -1;
            ListItem m_ListItem = dropDownList.Items.FindByText(text);
            dropDownList.SelectedValue = m_ListItem != null ? m_ListItem.Value : null;
        }
        public static void SelectDropDownListItemByValue(this DropDownList dropDownList, string text)
        {
            dropDownList.SelectedIndex = -1;
            ListItem m_ListItem = dropDownList.Items.FindByValue(text);
            dropDownList.SelectedValue = m_ListItem != null ? m_ListItem.Value : null;
        }

        public static List<T> FindControl<T>(Control parent) where T : Control
        {
            List<T> m_Controls = new List<T>();

            foreach (Control m_Control in parent.Controls)
            {
                if (m_Control is T || m_Control.GetType() == typeof(T))
                {
                    m_Controls.Add((T)m_Control);
                }
                m_Controls.AddRange(FindControl<T>(m_Control));
            }

            return m_Controls;
        }
        public static T FindParent<T>(Control control) where T : Control
        {
            T m_T = default(T);

            Control m_Parent = control.Parent;

            while (m_Parent != null)
            {
                if (m_Parent is T || m_Parent.GetType() == typeof(T))
                {
                    m_T = (T)m_Parent;
                    break;
                }
            }

            return m_T;
        }

        public static UserControl LoadControl(this Page page, string userControlPath, params object[] constructorParameters)
        {
            UserControl m_UserControl = null;
            try
            {
                List<Type> m_Types = new List<Type>();
                foreach (object constructorParameter in constructorParameters)
                {
                    m_Types.Add(constructorParameter.GetType());
                }

                m_UserControl = page.LoadControl(userControlPath) as UserControl;
                ConstructorInfo m_ContructorInfo = m_UserControl.GetType().BaseType.GetConstructor(m_Types.ToArray());

                m_ContructorInfo.Invoke(m_UserControl, constructorParameters);
            }
            catch (Exception ex)
            {
                Logging.LogMessage("Aeon.ManagementPortalUI.UserControlUtility:LoadControl", ex.ToString());
                m_UserControl = null;
            }

            return m_UserControl;
        }
        public static string GetFullUrl(string url)
        {
            string m_Result = string.Empty;
            Uri m_Uri = HttpContext.Current.Request.Url;
            try
            {
                if (url.StartsWith("/"))
                {
                    m_Result = m_Uri.Scheme + "://" + m_Uri.Host + url;
                }
                else if (url.StartsWith("http"))
                {
                    m_Result = url;
                }
                else
                {
                    m_Result = m_Uri.AbsoluteUri.Replace(m_Uri.AbsoluteUri.Substring(m_Uri.AbsoluteUri.LastIndexOf('/'), m_Uri.AbsoluteUri.Length - m_Uri.AbsoluteUri.LastIndexOf('/')), string.Empty) + "/" + url;
                }
            }
            catch (Exception ex)
            {
                m_Result = m_Uri.PathAndQuery;
                Logging.LogMessage("Aeon.ManagementPortalUI.UserControlUtility:GetFullUrl", ex.ToString());
            }
            return m_Result;
        }
    }
}
