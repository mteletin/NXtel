; version.asm
;
; Auto-generated by ZXVersion.exe
; On 28 Nov 2018 at 16:13

BuildNo                 macro()
                        db "132"
mend

BuildNoValue            equ "132"
BuildNoWidth            equ 0 + FW1 + FW3 + FW2



BuildDate               macro()
                        db "28 Nov 2018"
mend

BuildDateValue          equ "28 Nov 2018"
BuildDateWidth          equ 0 + FW2 + FW8 + FWSpace + FWN + FWo + FWv + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "16:13"
mend

BuildTimeValue          equ "16:13"
BuildTimeWidth          equ 0 + FW1 + FW6 + FWColon + FW1 + FW3



BuildTimeSecs           macro()
                        db "16:13:47"
mend

BuildTimeSecsValue      equ "16:13:47"
BuildTimeSecsWidth      equ 0 + FW1 + FW6 + FWColon + FW1 + FW3 + FWColon + FW4 + FW7
