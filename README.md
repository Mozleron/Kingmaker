# Unity Tech Challenge

You have been tasked with improving the Toplist micro-service. Add missing features, extend on existing functionality and make it overall more performant and elegant.

1. Complete the example, adding support to set Username, Score, Level-index.
2. Extend the toplist provider implmentation to:
 - Handle multiple toplists.
 - Cache entries across sessions.
 - Limit entry output by the maxEntries flag.
3. Finish and add additional Unit-tests, ensuring the functionality match and tests pass.
4. Add integration tests to the example.
5. The project should support both iOS and Android.

* **Finally** Explain shortly how you would distribute the toplist service to multiple external game teams. Focus on a stable and easily integrated delivery.
* **Note:** Only interfaces and features need to be the same, feel free to modify the implementation.
* **Extra credits:** Support toplists hosted on a remote server.


Delivery method:
I would compile this project as an Asset Bundle and then store it in some sort of asset store such as Artifactory or Nuget, or very worst case, 
a versioned directory on a network file share.  That way, developers can refer to the store for the version they want and can include it in
their projects as they desire.
