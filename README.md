# SimpleICS
Internet Connection Sharing (ICS). Console application to share internet connection to other network interface

<div align="center">
<p>
    <a href="https://github.com/gsirettito/SimpleICS/releases" target="_blank">
      <img alt="All releases" src="https://img.shields.io/github/downloads/gsirettito/NetworkToolBox/total.svg?style=for-the-badge&logo=github" />
    </a>
    <a href="https://github.com/gsirettito/SimpleICS/releases/latest" target="_blank">
      <img alt="Latest release" src="https://img.shields.io/github/downloads/gsirettito/NetworkToolBox/latest/total.svg?style=for-the-badge&logo=github" />
    </a>
    <a href="https://github.com/gsirettito/SimpleICS/releases" target="_blank">
      <img alt="Latest pre-release" src="https://img.shields.io/github/downloads-pre/gsirettito/NetworkToolBox/latest/total.svg?label=downloads%40pre-release&style=for-the-badge&logo=github" />
    </a>
  </p>
  <p>
    <a href="https://github.com/gsirettito/SimpleICS/stargazers" target="_blank">
      <img alt="GitHub stars" src="https://img.shields.io/github/stars/gsirettito/NetworkToolBox.svg?style=for-the-badge&logo=github" />
    </a>
    <a href="https://github.com/gsirettito/SimpleICS/network" target="_blank">
      <img alt="GitHub forks" src="https://img.shields.io/github/forks/gsirettito/NetworkToolBox.svg?style=for-the-badge&logo=github" />
    </a>
  </p>
  <p>
    <a href="https://github.com/gsirettito/SimpleICS/issues/new?labels=Feature-Request&template=Feature_request.md">
      <img alt="Feature request" src="https://img.shields.io/badge/github-feature_request-green.svg?style=for-the-badge&logo=github" />
    </a>
    <a href="https://github.com/gsirettito/SimpleICS/issues/new?labels=Issue&template=Bug_report.md">
      <img alt="Bug report" src="https://img.shields.io/badge/github-bug_report-red.svg?style=for-the-badge&logo=github" />
    </a>
  </p>
</div>

See https://www.codeproject.com/script/Articles/ArticleVersion.aspx?waid=4260144&aid=5326770 to more information

Commands:
- -l:                              Get all information for network adapters.
- -from "\<NetworkAdapterName>":   Name of the source network adapter that will share the connection to the destination.
- -to "\<NetworkAdapterName>":     Name of the destination network adapter with which you want to share the source connection.
- 1:                               Allows you to enable the source connection to the destination.
- 0:                               Allows you to disable the source connection to the destination.
  
  
Example:
  * This example enable connection to share internet from "Internet adapter" to "wlan adapter".
    
  `simpleics -from "Internet adapter" -to "wlan adapter" 1`
  * This example disable connection to share internet from "Internet adapter" to "wlan adapter".
    
  `simpleics -from "Internet adapter" -to "wlan adapter" 0`
  * This example disable SharingEnabled for this network adapter.
    
  `simpleics "Internet adapter" 0`
  * This example enables SharingEnabled for this network adapter by specifying that this is the origin.
    
  `simpleics "Internet adapter" 1 0`
  * This example enables SharingEnabled for this network adapter by specifying that this is the destination.
    
  `simpleics "Internet adapter" 1 1`
