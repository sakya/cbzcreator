# CBZ creator

A utility to create CBZ files from a bunch of folders containing images (like the one you obtain when downloading mangas using [HakuNeko](https://hakuneko.download/))

CBZ creator writes also the file `details.json` for [Tachiyomi](https://tachiyomi.org/)

## Gui
Using the GUI you can search for manga information and cover using [anilist.co](https://anilist.co/)

### Screenshots
![image](https://user-images.githubusercontent.com/289552/234328058-1f716126-8116-40e1-9948-546f546d2e35.png)

![image](https://user-images.githubusercontent.com/289552/234327998-30a1dded-a3bb-4ebf-aeea-a6ce16a67898.png)

## Command line
```
Usage:
CbzCreator.dll --input=VALUE --output=VALUE --title=VALUE [OPTIONS]


General:
  -r, --artist=VALUE          The artist
  -a, --author=VALUE          The author
  -c, --coverurl=VALUE        The cover url
  -d, --description=VALUE     The description
  -g, --genre=VALUE           The genre
                              This option can be set multiple times
  -i, --input=VALUE           The path containing all the comic books
  -o, --output=VALUE          The path where the cbz files will be created
  -t, --title=VALUE           The title

```

