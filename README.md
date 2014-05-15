## Introduction #

TcmDeployer is a simple drop-in replacement for the SDL Tridion content deployer service.
It was mainly created to be able to run a deployer with a seperate set of Tridion libraries and configuration than the system installed version.

### Download #

TcmDeployer downloads can be found here:
[TcmDeployer on Google Drive](https://drive.google.com/folderview?id=0B7HbFVRJj_Unc1o3M2RCU3JWYW8&usp=sharing TcmDeployer on Google Drive)


### Issue Experienced #

When running the Tridion content deployer, it will always load the Tridion JAR libaries from the Tridion home folder.

The Tridion home folder is determined in order of priority:

 * Current [HttpContext](http://msdn.microsoft.com/en-us/library/system.web.httpcontext(v=vs.110).aspx) (if !HttpContext is present) ~/bin folder
 * Current Directory
 * "TRIDION_HOME" value from registry 
     * "Software\Tridion\Content Delivery\General"
     * "Software\Wow6432Node\Tridion\Content Delivery\General"
 * Environment variable "TRIDION_HOME"
 * "C:\Program Files\Tridion"

Any of the above folders is considered valid if the following files are present: 

 * <folder>\lib\cd_core.jar
 * <folder>\lib\cd_model.jar
 * <folder>\config\cd_broker_conf.xml _OR_ <folder>\config\cd_storage.conf.xml

When running an in-process deployer in combination with an ASP.NET website there is no issue since the JAR libraries deployed as part of the website will be evaluated first.

However when running a separate deployer service instance it is not possible to point the service to a preferred set of JARs.
Rather it will default to the standard Tridion installation on the machine.

### Resolution #

TcmDeployer will automatically set the current directory to the executing assembly directory, ensuring that Tridion will validate that folder a valid Tridion home folder first.

Additionally it allows to be ran from the Windows command prompt, the original cd_deployer.exe included with Tridion was designed to do this but somehow it does not work.

Also all the parameters work as per the Tridion documentation:

    Usage: cd_deployer (-start|-install [instance-id instance-config-directory]|-remove [instance-id])


The "-start" option does not take into account a specified instance id and instance configuration directory in the original cd_deployer.
