﻿﻿Naming convention for namespaces and classes with either IGet or IPost should be snake_case.
This is because reflection will be used to construct the url for the api call.
So api/hello-world/print will access the class print in the namespace api.hello_world.
Only public classes with IGet and/or IPost will be handled.