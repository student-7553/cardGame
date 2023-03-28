namespace SingularityGroup.HotReload {
    internal static class PackageConst {
        public const string Version = "1.8.0";
        // Never higher than Version
        // Used for the download
        public const string ServerVersion = "1.8.0";
        public const string PackageName = "com.singularitygroup.hotreload";
        public const string LibraryCachePath = "Library/" + PackageName;
        public const string ConfigFileName = "hot-reload-config.json";
    }
}
