PineFramework
=============

Add life to numbers.


What is PINE?
-----

PINE stands for "Procedural Iterative Numerical Effects". With PINE, you can easily incorporate complex mathematical operations into game objects.

PINE uses an Assembly-like language to program different kinds of effects. It has plenty of built-in math functions to make creating effects as simple as possible.


Here are just a few examples of what PINE can do:

* Interpolation
* Lighting effects; such as sinusoids, flashing, fading and flickering
* Control particle motion
* Apply realistic movements to first-person cameras
* Sound modulation
* Apply animations to menu elements


Effect code examples
-----

Basic slide from 0-1:

```
push x
pop out
```

Sine wave:
```
push x
push pi
push 2
prd 3
sin
pop out
```

Random number from 0-9
```
push 0
push 10
rand
pop out
```

Triangle wave:
```
push x
push 0.5
ge
not
copy
pop a
jnz up
push 1
#up
push x
push a
jnz down
sub
#down
pop out
```

Flame flickering effect
```
push c
jnz start
push 1
pop c

push h
push 0
gt
jnz start
push 1
pop h

#start
push 0
push 1
rand
push 0.7
gt

push b

push 0
push 1
rand

lerp
pop b

push b
push a
sub
push 0.2
mul
push a
add
copy
pop a

push h
mul
pop out
```
