Cabal Online Updater
=====================
A sample game updater/launcher made for test servers. This code can be used as a starting point for building your custom better verion of the updater.

PS: Not all exceptions/issues have been gracefully handled. Hence be careful while using it on your live server!

Features
---------
* 7zip (LZMA) compression used to keep patch files smaller in size.
* Has full check as well as patch check functionality.
* Target dot net version is 2.0 so that players need not install latest dot net frameworks in order to run the updater.
* Includes a patch generator to quickly build a patch for your game.
* Updater is very simple lightweight 297KB executable without unnecessary functionalities.

Basic Usage
------------
* Open the solution in Visual Studio 2017 or above.
* Update the ``PatchHost`` as well as ``WebHost`` variables in Updater project to your domain or IP address (It's 127.0.0.1 by default)
* Build both updater as well as patcher projects with x86 target platform.
* Open ``CabalOnlinePatchGenerator.exe`` and generate patch twice. First time with all game files in the input directory and once done rename generated ``patch.ini`` to ``full.ini``. Second time with only important files you want to check each time updater starts.
* Upload all the generated files into the patch directory of your web server. In order to updater to work as intended ``http://yourwebserver/patch/full.ini`` as well ``http://yourwebserver/patch/patch.ini`` should exist.

Known Issues
-------------
* As mentioned before not all errors have been handled properly and hence updater might crash if unhandled exception occurs.
* Sometime file download progress bar gets stuck while downloading files.
* Full check takes hours to finish if client folder has no previous cabal game files as multipart file download and multi-thread file processing has not been implemented yet.

Report errors with screenshots in the issues section of this project.
