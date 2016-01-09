# TBID
Lightweight image downloader for tumblr blogs.
I'm still going to update this later.

# General Information
TBID is a lightweight application that allows one to download images from a tumblr blog.  
Download settings can be established, such as image count limits and tags to further specify the images to download.  
To use this application, you **must have a valid tumblr API key** (might change this in the future).

# Usage
To use TBID, simply enter the correct information in the fields and click the "Start" button.

1. Username or Blog Link: Enter the username or blog link of the blog you would like to download from.  
If a valid URL is not given, it will be assumed that the input is a username; the protocol / server name (*http://* and *www*) will be added to the front of the input and *.tumblr.com* will be appended to the end of it.
2. Tags to use: Enter a tag (or multiple tags, separated with spaces) to use during the download. All images downloaded will contain the tag(s) inputted.  
**Do NOT include the tag character** (#).  
If this is blank (or only whitespace) then no tags will be used in downloading.
3. Download Directory: Input the directory to download the images. To open a folder browser dialog, click the "..." button next to the text box. You can also insert the directory manually (if, for whatever reason, the folder browser dialog does not work).
4. Download limit: This is a number that controls how many images TBID will download. If this is set to a positive nonzero number, TBID will only download that amount of pictures. If this is set to 0, it will download all images from the blog.  
Pictures not downloaded due to already existing in the List Cache and the List Cache is enabled do not count towards this download limit (see below).
5. Use List Cache: If this is checked, a List Cache will be created (if it doesn't exist already) for each blog you download from. A List Cache, whilst the option is enabled, will contain all the links to images downloaded. As images are downloaded, their links will be added to the List Cache. Any images which have links that exist in the List Cache will not be downloaded and will not be tallied towards the Download Limit (if a positive nonzero value is set).
6. Notify when download finishes: If this is checked, a notification in your system tray will be displayed notifying you when a download has completed.
7. Your API Key: You must enter a valid API key to utilize this program. This field will display all characters as black password dots (•).

# Screenshot(s)
![alt-text](http://i.imgur.com/X8Rf0GF.png "TBID Main Window")

# Todo
* Include possible errors / exceptions
* Include more screenshots if needed

# Other stuff
I'll update this more... later.
