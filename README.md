# CBZ creator

A utility to create CBZ files from a bunch of folders containing images (like the one you obtain when downloading mangas using [HakuNeko](https://hakuneko.download/))

CBZ creator writes also the file `details.json` for [Tachiyomi](https://tachiyomi.org/)

The input folder is the one containing all the volums/capters subfolder and shoould have a structure like this:
```
/input/Manga title
 |
 +- Vol 1
 |    001.jpg
 |    002.jpg
 |    ...
 +- Vol 2
 |    001.jpg
 |    002.jpg
 |    ...
```

In the output folder all the cbz will be created and will have a structure like this:
```
/output/Manga title
 | cover.jpg
 | details.json
 | Manga title - Vol 1.cbz
 | Manga title - Vol 2.cbz
 | ...
```

## Gui
Using the GUI you can search for manga information and cover using [anilist.co](https://anilist.co/)

### Screenshots
![image](https://user-images.githubusercontent.com/289552/235618354-0720f82d-1389-4b30-8261-5fda1a178045.png)


![image](https://user-images.githubusercontent.com/289552/235618406-42547049-9f45-4591-a015-855beb4bc37c.png)

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

