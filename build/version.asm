; version.asm
;
; Auto-generated by ZXVersion.exe
; On 12 Jul 2018 at 17:37

BuildNo                 macro()
                        db "11"
mend

BuildNoValue            equ "11"
BuildNoWidth            equ 0 + FW1 + FW1



BuildDate               macro()
                        db "12 Jul 2018"
mend

BuildDateValue          equ "12 Jul 2018"
BuildDateWidth          equ 0 + FW1 + FW2 + FWSpace + FWJ + FWu + FWl + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "17:37"
mend

BuildTimeValue          equ "17:37"
BuildTimeWidth          equ 0 + FW1 + FW7 + FWColon + FW3 + FW7



BuildTimeSecs           macro()
                        db "7/12/2018 5:37:03 PM"
mend

BuildTimeSecsValue      equ "7/12/2018 5:37:03 PM"
BuildTimeSecsWidth      equ 0 + FW7 + FW/ + FW1 + FW2 + FW/ + FW2 + FW0 + FW1 + FW8 + FWSpace + FW5 + FWColon + FW3 + FW7 + FWColon + FW0 + FW3 + FWSpace + FWP + FWM