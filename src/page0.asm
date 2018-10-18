; page0.asm

Page0Temp16  equ $
Page0Start32 equ Ringo
Page0Start16 equ Page0Start32
org              Page0Start32

// This will always appear at position $9000 in the .SNA file
org $D000-$1B

ResourcesCount:         db Resources.Count
PagesCount:             db Pages.Count

FileName:
  db "NXtel.sna", 0
  ds 31-($-FileName), 0                                 ; Filename can be up to 30 letters

Resources proc Table:

  ; Bank  FName  Index  Notes
  db  30,    30  ;   0  Layer 2 Teletext renderer
//  db  31,    31  ;   1  Pages31.P0 to Pages31.P7
  db  32,    32  ;   2  Pages33.P0 to Pages33.P7
  db  33,    33  ;   2  Pages33.P0 to Pages33.P7
  db  34,    34  ;   3  Pages34.P0 to Pages34.P7
  db  35,    35  ;   4  Pages35.P0 to Pages35.P7
  db  36,    36  ;   5  Pages36.P0 to Pages36.P7
  db  37,    37  ;   6  Pages37.P0 to Pages37.P7
  db  38,    38  ;   7  Pages38.P0 to Pages38.P7

  struct
    Bank        ds 1
    FName       ds 1
  Size send

  Len           equ $-Table
  Count         equ Len/Size
  ROM           equ 255

  output_bin "..\build\BankList.bin", Table, Len
pend



Pages proc Table:

  ; Bank  Slot  Duration    Notes

//  db  31,    0, dw 250+NOC  ;   Title
//  db  31,    1, dw 150+NOC  ;   Title
//  db  31,    2, dw 200+NOC  ;   Title
//  db  31,    1, dw 500+NOC  ;   Title

  db  37,    5, dw 350+NOC  ;   Title
  db  37,    7, dw  50+NOC  ;   Blank

  db  32,    0, dw 200+CLK  ;   0
  db  32,    1, dw 200+CLK  ;   1
  db  32,    2, dw 200+CLK  ;   2
  db  32,    3, dw 200+CLK  ;   3
  db  32,    4, dw 200+NOC  ;   4
  db  32,    5, dw 200+NOC  ;   5
  db  32,    6, dw 200+NOC  ;   6
  db  32,    7, dw 200+CLK  ;   7

  db  33,    0, dw 200+NOC  ;   0
  db  33,    1, dw 200+CLK  ;   1
  db  33,    2, dw 200+NOC  ;   2
  db  33,    3, dw 200+CLK  ;   3
  db  33,    4, dw 100+CLK  ;   4
  db  33,    5, dw 100+CLK  ;   5
  db  33,    6, dw 100+CLK  ;   6
  db  33,    7, dw 100+NOC  ;   7

  db  34,    0, dw 200+CLK  ;   0
  db  34,    1, dw 100+CLK  ;   1
  db  34,    2, dw 100+CLK  ;   2
  db  34,    3, dw 100+CLK  ;   3
  db  34,    4, dw 100+CLK  ;   4
  db  34,    5, dw 100+CLK  ;   5
  db  34,    6, dw 100+CLK  ;   6
  db  34,    7, dw 100+CLK  ;   7

  db  35,    0, dw 100+CLK  ;   0
  db  35,    1, dw 100+CLK  ;   1
  db  35,    2, dw 100+CLK  ;   2
  db  35,    3, dw 100+CLK  ;   3
  db  35,    4, dw 100+CLK  ;   4
  db  35,    5, dw 100+CLK  ;   5
  db  35,    6, dw 200+NOC  ;   6
  db  35,    7, dw 200+NOC  ;   7

  db  36,    0, dw 200+NOC  ;   0
  db  36,    1, dw 200+NOC  ;   1
  db  36,    2, dw 200+NOC  ;   2
  db  36,    3, dw 200+NOC  ;   3
  db  36,    4, dw 200+NOC  ;   4
  db  36,    5, dw 200+NOC  ;   5
  db  36,    6, dw 200+NOC  ;   6
  db  36,    7, dw 200+NOC  ;   7

  db  37,    0, dw 200+NOC  ;   0
  db  37,    1, dw 200+NOC  ;   1
  db  37,    2, dw 200+NOC  ;   2
  db  37,    3, dw 200+NOC  ;   3
  db  37,    4, dw 200+NOC  ;   4
  db  37,    7, dw 100+NOC  ;   7
  db  37,    6, dw 350+NOC  ;   5
  db  37,    7, dw 100+NOC  ;   7

  struct
    Bank        ds 1
    Slot        ds 1
    Duration    ds 2
  Size send

  Len           equ $-Table
  Count         equ Len/Size

  CLK           equ %1000 0000 0000 0000
  NOC           equ %0000 0000 0000 0000
pend



Page0End32   equ $-1
Page0End16   equ Page0End32
Page0Size equ Page0End32-Page0Start32+1
if Page0Size<>(Page0End16-Page0Start16+1)
  zeuserror "Page0Size calculation error"
endif
org Page0Temp16
