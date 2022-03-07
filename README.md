# SimpleICS
Internet Connection Sharing (ICS). Console application to share internet connection to other network interface

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
