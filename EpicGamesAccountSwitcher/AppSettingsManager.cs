using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesAccountSwitcher
{
    internal class AppSettingsManager
    {
        //public static IAccountInfoCollection Read(string path)
        //{
        //    StreamReader stream = File.OpenText(path);
        //    string line = stream.ReadLine();

        //    if (!line.StartsWith("Name:"))
        //    {
        //        throw new ParseException("Het bestand is corrupt; moet beginnen met 'Name:'");
        //    }

        //    accountInfoCollection.Name = line.Substring(line.IndexOf(':'));

        //    line = stream.ReadLine();
        //    while (line != null)
        //    {
        //        string[] accountParts = line.Split(',');

        //        string title = accountParts[0];
        //        string username = accountParts[1];
        //        string password = accountParts[2];
        //        string notes = accountParts[3];
        //        DateTime expirationDate = Convert.ToDateTime(accountParts[4]);

        //        AccountInfo accountInfo = new AccountInfo
        //        {
        //            Title = title,
        //            Username = username,
        //            Password = password,
        //            Notes = notes,
        //            Expiration = expirationDate
        //        };

        //        accountInfoCollection.AccountInfos.Add(accountInfo);
        //        line = stream.ReadLine();
        //    }

        //    stream.Close();
        //    return accountInfoCollection;
        //}
    }
}
