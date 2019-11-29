/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System.Reflection;
using System.Runtime.InteropServices;
using YetaWF.Core.Packages;
using YetaWF.PackageAttributes;

[assembly: AssemblyTitle("DockerRegistry")]
[assembly: AssemblyDescription("DockerRegistry description")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Softel vdm, Inc.")]
[assembly: AssemblyProduct("DockerRegistry")]
[assembly: AssemblyCopyright("Copyright © 2019 - Softel vdm, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("5.0.0.0")]

[assembly: Package(PackageTypeEnum.Module, "YetaWF")]
[assembly: PackageInfo("https://YetaWF.com/UpdateServer",
    "https://YetaWF.com/Documentation/YetaWF/DockerRegistry",
    "https://YetaWF.com/Documentation/YetaWF/Support",
    "https://YetaWF.com/Documentation/YetaWF/DockerRegistry#Release%20Notice",
    "https://YetaWF.com/Documentation/YetaWF/DockerRegistry#License")]

[assembly: RequiresPackage("YetaWF.ComponentsHTML")] // needed for HTML components (can be removed if no views/components are implemented by this package)

// https://docs.docker.com/registry/garbage-collection/
// docker exec registry bin/registry garbage-collect --dry-run /etc/docker/registry/config.yml


// https://forums.docker.com/t/get-image-digest-from-remote-registry-via-api/9480
// https://github.com/docker/distribution/issues/1565
