using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace ProjectManagementSystem.Common
{

    public class SafePassword
    {
        private static int STRETCH_COUNT = 1000;

        /// <summary>
        /// salt＋ハッシュ化したパスワードを取得
        /// </summary>
        public static string GetSaltedPassword(string password)
        {
            string salt = GetSha256(ConfigurationManager.AppSettings[ConfigurationKeys.ENCRYTION_STRING]);
            return GetSha256(salt + password);
        }

        /// <summary>
        /// salt + ストレッチングしたパスワードを取得(推奨)
        /// </summary>
        public static string GetStretchedPassword(string password, string userId)
        {
            string salt = GetSha256(userId);
            string hash = "";

            for (int i = 0; i < STRETCH_COUNT; i++)
            {
                hash = GetSha256(hash + salt + password);
            }

            return hash;

        }

        /// <summary>
        /// 文字列から SHA256 のハッシュ値を取得
        /// </summary>
        public static String GetSha256(string target)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] byteValue = Encoding.UTF8.GetBytes(target);
            byte[] hash = mySHA256.ComputeHash(byteValue);

            StringBuilder buf = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                buf.AppendFormat("{0:x2}", hash[i]);
            }

            return buf.ToString();
        }
    }
}
