; version.asm
;
; Auto-generated by ZXVersion.exe
; On 07 Oct 2018 at 11:26

BuildNo                 macro()
                        db "59"
mend

BuildNoValue            equ "59"
BuildNoWidth            equ 0 + FW5 + FW9



BuildDate               macro()
                        db "07 Oct 2018"
mend

BuildDateValue          equ "07 Oct 2018"
BuildDateWidth          equ 0 + FW0 + FW7 + FWSpace + FWO + FWc + FWt + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "11:26"
mend

BuildTimeValue          equ "11:26"
BuildTimeWidth          equ 0 + FW1 + FW1 + FWColon + FW2 + FW6



BuildTimeSecs           macro()
                        db "10/7/2018 11:26:43 AM"
mend

BuildTimeSecsValue      equ "10/7/2018 11:26:43 AM"
BuildTimeSecsWidth      equ 0 + FW1 + FW0 + FW/ + FW7 + FW/ + FW2 + FW0 + FW1 + FW8 + FWSpace + FW1 + FW1 + FWColon + FW2 + FW6 + FWColon + FW4 + FW3 + FWSpace + FWA + FWM
