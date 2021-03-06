## Space Invaders [![Build Status](https://travis-ci.com/Miguelito79/SpaceInvaders.svg?branch=master)](https://travis-ci.com/Miguelito79/SpaceInvaders)

A space invaders emulator written in c#.  
The CPU is fully implemented including the DAA instruction and the auxillary carry flag.  


![enter image description here](https://github.com/Miguelito79/SpaceInvaders/blob/master/Images/screenshot.jpg)

 ### HOW TO PLAY
 
|Key| Action  |
|--|--|
|C  |Insert Coin  |
|p  |Pause / Resume |
|1  |Start Player 1  |
|Arrow Left| P1 Move left|
|Arrow Right| P1 Move right|
|Arrow Up | P1 Fire|
|2 | Start Player 2|
|S | P2 Move left|
|F | P2 Move right
|E | P2 Fire|



 ### TEST RESULTS

 **CPUTEST.COM - DIAGNOSTICS II V1.2 - CPU TEST** 

    DIAGNOSTICS II V1.2 - CPU TEST
    COPYRIGHT (C) 1981 - SUPERSOFT ASSOCIATES

    ABCDEFGHIJKLMNOPQRSTUVWXYZ
    CPU IS 8080/8085
    BEGIN TIMING TEST
    END TIMING TEST
    CPU TESTS OK  

**TST80.COM**

    MICROCOSM ASSOCIATES 8080/8085 CPU DIAGNOSTIC
    VERSION 1.0  (C) 1980

    CPU IS OPERATIONAL

**8080PRE.COM Preliminary intel 8080 tests**

    8080 Preliminary tests complete
    
**8080exer.COM Intel 8080 instruction set exerciser**

    8080 instruction exerciser
    dad <b,d,h,sp>................  OK
    aluop nn......................  OK
    aluop <b,c,d,e,h,l,m,a>.......  OK
    <daa,cma,stc,cmc>.............  OK
    <inr,dcr> a...................  OK
    <inr,dcr> b...................  OK
    <inx,dcx> b...................  OK
    <inr,dcr> c...................  OK
    <inr,dcr> d...................  OK
    <inx,dcx> d...................  OK
    <inr,dcr> e...................  OK
    <inr,dcr> h...................  OK
    <inx,dcx> h...................  OK
    <inr,dcr> l...................  OK
    <inr,dcr> m...................  OK
    <inx,dcx> sp..................  OK
    lhld nnnn.....................  OK
    shld nnnn.....................  OK
    lxi <b,d,h,sp>,nnnn...........  OK
    ldax <b,d>....................  OK
    mvi <b,c,d,e,h,l,m,a>,nn......  OK
    mov <bcdehla>,<bcdehla>.......  OK
    sta nnnn / lda nnnn...........  OK
    <rlc,rrc,ral,rar>.............  OK
    stax <b,d>....................  OK
    Tests complete

### Resources

 - [Emulator101](http://emulator101.com/)
 - [Intel 8080 Assembly Lanuage Programming Manual](https://altairclone.com/downloads/manuals/8080%20Programmers%20Manual.pdf)
 - [AltairClone CPU Test](https://altairclone.com/downloads/cpu_tests/)
 
