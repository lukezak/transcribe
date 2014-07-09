Transcribe
========

Transcribe is a tool used for transcribing digitised paper documents. See more at http://lukezak.blogspot.com.au/ or check out the demo at http://lukepzak.net

Prerequisites
-----
+ Visual Studio 2013
+ MVC 5
+ Tesseract open source OCR engine

Installation
-----
1. Open the project in Visual Studio 2013
2. Make any necessary changes to the tool e.g. text, theme, design etc.
3. Publish site to a hosted web server (IIS)
4. Review web.config and check connection strings, app settings etc. Recommend changing the 'intialAdminPassword' parameter.
5. Copy Tesseract open source OCR engine over to the top level directory or where specified in the web.config.
6. Create '_upload' and '_pdf' folder in the 'Content' directory or where specified in the web.config. Assign write permissions to these folders.
7. Launch the website and register a user to seed the Administrator account.
8. Log in as Administrator (admin@transcribe.com) to upload documents to be transcribed. To upload documents select the 'Hello Admin' button in the top right hand corner, click the link to the 'Administration area' and select the 'Upload' button. Follow the wizard to upload a document.

Author
-----
Luke Pasturczak

+ http://twitter.com/lukepzak/
+ http://github.com/lukezak/
+ http://lukezak.blogspot.com.au/

Copyright and License
----
Copyright Luke Pasturczak 2014

Code released under the MIT License.
