# Batch Image Processor #
[![Batch Image Processor](http://imgur.com/7j6L5ok.png)](http://imgur.com/7j6L5ok.png)
## What does it Do? ##

Import hundreds of photos and perform various manipulations on them!
* Rotate
  * Angular
  * Auto-Portrait
  * Auto-Landscape
* Resize
  * Smart resizing, matches the destination size to the aspect ratio of your images
* Crop
* Watermarks
  * Text Watermark
  * Image Watermarks
  * Variable Opacity
* Adjustments
  * Brightness
  * Contrast
  * Gamma
  * Saturation, or
    * Greyscale / Sepia
* Rename

And more!

## CLI ##

Now includes a complete CLI interface! Usage:

`BatchImageProcessor.exe [flags] [file0 file1 ... fileX]`

Including files in the command will process each file listed. For large numbers of files, it may be easier to use a manifest file (see `--man`). Specifying any flags (except `-s` and `-e`) or files will disable the GUI.

#### Flags: ####
Batch Image Processor CLI uses flags to specify options. Short flags, like `-s` and `-e`, can be combined as `-se`.

 * `--man="file"`: Specify a manifest file (a plain text document containing a raw list of files).
 * `--rotate=#`: A rotation transform.
  * `0` = None (Default)
  * `1-3` = Clockwise rotation in 90-degree increments.
  * `4` = Auto-Portrait Orientation.
  * `5` = Auto-Landscape Orientation.
 * `--resize=#`: A resize transform.
  * `0` = None (Default)
  * `1` = Resize the image to be smaller than the specified by `rwidth` and `rheight`.
  * `2` = Resize the image to be larger than the specified by `rwidth` and `rheight`.
  * `3` = Resize the image to be the exact size specified by `rwidth` and `rheight`.
 * `--rwidth=#`: Width parameter for the resize option.
 * `--rheight=#`: Height parameter for the resize option.
 * `-a` or `--noaspect`: Disable automatic aspect ratio matching when resizing.
 * `-c` or `--crop`: Enable cropping.
 * `--cwidth=#`: Width parameter for the crop option.
 * `--cheight=#`: Height parameter for the crop option.
 * `--calign=#`: Crop alignment.
  * `0   1   2`
  * `3   4   5`
  * `6   7   8`
 * `-w` or `--watermark`: Enable watermarking.
 * `--wtype=(Text|Image)`: Specifies the watermark type. Case sensitive.
  * `Text` = A text watermark. Additional options:
   * `--wfont=fontname`: Specify the watermark font.
   * `--wsize`: Specify the point-size of the watermark font.
   * `--wparam="Text"`: The text to overlay.
  * `Image` = An image watermark. Additional options:
   * `--wcolor`: Image watermarks in color (instead of default grayscale)
 * `--wopac=#.#`: Watermark opacity as a decimal. E.g. `--wopac=0.4` = 40%
 * `--walign=#`: Watermark alignment.
  * `0   1   2`
  * `3   4   5`
  * `6   7   8`
 * `--brightness=#.#`: Adjust image brightness as a decimal.
 * `--contrast=#.#`: Adjust image contrast as a decimal.
 * `--gamma=#.#`: Adjust image gamma as a decimal.
  * Minimum 0.1, maximum 5.0
 * `--smode=#`: Saturation mode.
  * `0` = Saturation (Default)
  * `1` = Grayscale
  * `2` = Sepia
 * `--saturation=#.#`: Adjust image saturation as a value. `--saturation=0` is the same as `--smode=1`
 * `--output="dir"`: Specify the output directory. If one is not specified files will be output to the current working directory.
 * `--format=(Jpg|Png|Bmp|Gif|Tiff)`: Specify the output filetype. Case sensitive. Default is Jpg.
 * `-?` or `--help`: Display the help info (similar to this page).

#### GUI Flags ####
In addition to CLI flags, there are also some GUI flags:
* `-s` or `--noshaders`: Disable the drop shadow and blur shaders used in the GUI
* `-e` or `--noaero`: Disable the Windows Aero extensions
