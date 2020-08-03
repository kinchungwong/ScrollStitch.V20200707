FixedWidthFontExtractionTool.exe
===

Usage
===

Usage 1: print a character map template (as text) to the console or saved to a file.
=====

```
    FixedWidthFontExtractionTool.exe PrintTemplate {text_template.txt}
```

If the output text file is omitted, the text template is printed to the console.

---

Usage 2: parse a screenshot of the text template and extract the fixed-width fonts as a resource.
=====

```
    FixedWidthFontExtractionTool.exe ProcessImage {screenshot.png}
```

The processed output is saved to the same directory as the screenshot image.
The following outputs are generated:

- {screenshot}_CharBitmap.png
- {screenshot}_Base64.txt
- {screenshot}_CodeFragment.txt
