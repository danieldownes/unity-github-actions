using UnityEngine;

namespace Stroll.Assets.Editor.BuildTools
{
    class BuildConfig
    {
        [SerializeField]
        private string applicationIdentifier;

        [SerializeField]
        private string companyName;

        [SerializeField]
        private string productName;

        [SerializeField]
        private bool isChannelApp;

        [SerializeField]
        private bool signPackage;

        [SerializeField]
        private string certificatePath;

        [SerializeField]
        private string mlSdkPath;

        [SerializeField]
        private string versionName;

        [SerializeField]
        private int versionCode;

        [SerializeField]
        private string metroCertificatePath;

        internal string GetApplicationIdentifier()
        {
            return this.applicationIdentifier;
        }
        internal string GetCompanyName()
        {
            return this.companyName;
        }
        internal string GetProductName()
        {
            return this.productName;
        }
        internal bool GetIsChannelApp()
        {
            return this.isChannelApp;
        }
        internal bool GetSignPackage()
        {
            return this.signPackage;
        }
        internal string GetCertificatePath()
        {
            return this.certificatePath;
        }
        internal string GetMlSdkPath()
        {
            return this.mlSdkPath;
        }
        internal string GetVersionName()
        {
            return this.versionName;
        }
        internal int GetVersionCode()
        {
            return this.versionCode;
        }
        internal string GetMetroCertificatePath()
        {
            return this.metroCertificatePath;
        }
    }
}