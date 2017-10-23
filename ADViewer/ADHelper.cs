using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections;

namespace ADViewer
{
    public class ADHelper
    {
        public User GetUserModel(InputTypes inputType, string inputValue)
       {
            var user = new User();

            user.Properties = GetPropertyModels(inputType, inputValue);

            string strSAMAccountName = user.Properties.Find(p => p.Property.ToLower() == "samaccountname").Value;

            user.Groups = GetUserGroups(strSAMAccountName);

            return user;
        }

        public List<ADModel> GetPropertyModels(InputTypes inputType, string inputValue)
        {
            var list = new List<ADModel>();

            try
            {
                using (var de = new DirectoryEntry())//("LDAP://DC=corpusers,dc=net"))
                {
                    using (var adSearch = new DirectorySearcher(de))
                    {
                        var strFilter = GetSearchFilter(inputType, inputValue);
                        if (string.IsNullOrEmpty(strFilter))
                            return null;
                        adSearch.Filter = strFilter;
                        SearchResult adSearchResult = adSearch.FindOne();
                        if (adSearchResult == null)
                        {
                            return null;
                        }

                        foreach (System.Collections.DictionaryEntry item in adSearchResult.Properties)
                        {
                            if (string.IsNullOrEmpty(item.ToString()))
                            {
                                continue;
                            }
                            var model = new ADModel();
                            model.Property = item.Key.ToString();
                            model.Value = ((ResultPropertyValueCollection)item.Value)[0].ToString();
                            list.Add(model);
                        }
                    }
                }
                list.Sort();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetUserGroups(string SAMAccountName)
        {
            var list = new List<string>();

            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
                {
                    using (var p = Principal.FindByIdentity(ctx, SAMAccountName))
                    {
                        if (p == null)
                            return list;

                        var groups = p.GetGroups();
                        using (groups)
                        {
                            foreach (Principal g in groups)
                            {
                                list.Add(g.Name);
                            }
                            list.Sort();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return list;
        }

        public string GetSearchFilter(InputTypes inputType, string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            
            //return "(anr=" + input + ")";

            switch (inputType)
            {
                case InputTypes.ID:
                    return string.Format("(SAMAccountName={0})", input);
                case InputTypes.Email:
                    return string.Format("(mail={0})", input);
                case InputTypes.Name:
                    return string.Format("(cn={0})", input);
                default:
                    return "(anr=" + input + ")";
            }

        }
    }
}
