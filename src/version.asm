; version.asm
;
; Auto-generated by ZXVersion.exe
; On 26 Nov 2018 at 22:10

BuildNo                 macro()
                        db "131"
mend

BuildNoValue            equ "131"
BuildNoWidth            equ 0 + FW1 + FW3 + FW1



BuildDate               macro()
                        db "26 Nov 2018"
mend

BuildDateValue          equ "26 Nov 2018"
BuildDateWidth          equ 0 + FW2 + FW6 + FWSpace + FWN + FWo + FWv + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "22:10"
mend

BuildTimeValue          equ "22:10"
BuildTimeWidth          equ 0 + FW2 + FW2 + FWColon + FW1 + FW0



BuildTimeSecs           macro()
                        db "22:10:24"
mend

BuildTimeSecsValue      equ "22:10:24"
BuildTimeSecsWidth      equ 0 + FW2 + FW2 + FWColon + FW1 + FW0 + FWColon + FW2 + FW4
