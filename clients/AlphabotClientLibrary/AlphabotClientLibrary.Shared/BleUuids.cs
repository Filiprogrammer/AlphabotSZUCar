using System;

namespace AlphabotClientLibrary.Shared
{
    public static class BleUuids
    {
        public static readonly Guid DRIVE_STEER = new Guid("a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2");

        public static readonly Guid PINGCLIENT = new Guid("117ad3a5-b257-4465-abd4-7dc12a4cf77d");

        public static readonly Guid TOGGLE = new Guid("fce001d4-864a-48f4-9c95-de928f1da07b");

        public static readonly Guid SENSOR = new Guid("4c999381-35e2-4af4-8443-ee8b9fe56ba0");

        public static readonly Guid NAVIGATION_TARGET = new Guid("f56f0a15-52ae-4ad5-bfe1-557eed983618");

        public static readonly Guid CALIBRATE = new Guid("d39e8d54-8019-46c8-a977-db13871bac59");

        public static readonly Guid ADD_OBSTACLE = new Guid("60db37c7-afeb-4d40-bb17-a19a07d6fc95");

        public static readonly Guid REMOVE_OBSTACLE = new Guid("6d43e0df-682b-45ef-abb7-814ecf475771");

        public static readonly Guid PATH_FINDING_PATH = new Guid("8dad4c9a-1a1c-4a42-a522-ded592f4ed99");

        public static readonly Guid ANCHOR_LOCATIONS = new Guid("8a55dd30-463b-40f6-8f21-d68efcc386b2");

        public static readonly Guid ERROR = new Guid("dc458f08-ea3e-4fe1-adb3-25c840be081a");
    }
}
