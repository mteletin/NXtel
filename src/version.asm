; version.asm
;
; Auto-generated by ZXVersion.exe
; On 12 Mar 2019 at 22:50

BuildNo                 macro()
                        db "219"
mend

BuildNoValue            equ "219"
BuildNoWidth            equ 0 + FW2 + FW1 + FW9



BuildDate               macro()
                        db "12 Mar 2019"
mend

BuildDateValue          equ "12 Mar 2019"
BuildDateWidth          equ 0 + FW1 + FW2 + FWSpace + FWM + FWa + FWr + FWSpace + FW2 + FW0 + FW1 + FW9



BuildTime               macro()
                        db "22:50"
mend

BuildTimeValue          equ "22:50"
BuildTimeWidth          equ 0 + FW2 + FW2 + FWColon + FW5 + FW0



BuildTimeSecs           macro()
                        db "22:50:54"
mend

BuildTimeSecsValue      equ "22:50:54"
BuildTimeSecsWidth      equ 0 + FW2 + FW2 + FWColon + FW5 + FW0 + FWColon + FW5 + FW4
