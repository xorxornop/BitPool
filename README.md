BitPool
=======

Combination shared mailbox + mix network + remailer service with wider connectivity over BitMessage P2P network

Rationale
---------

BitMessage requires clients to prospectively attempt to decrypt all messages, making it rather unsuitable for mobile clients, which - annoyingly - is one of the if not the most important potential applications for it.

When people communicate it it often in smaller groups than that of the entire world, so allowing some congregation into the smaller networks the vast majority of us inhabit - and so with it shrink computational and bandwidth requirements with it - seems a natural choice. At the same time, it would be nice to not give up too much anonymity.

BitPool is designed to do just that, through allowing users to join whatever pool(s) they like. If one is primarily concerned with optimising for the use case of talking primary to their circle of contacts, then one can set up a pool for that. Otherwise, or and, they may join a random pool, and so gain some additional anonymity.

The pool should allow them to send messages to other members of the pool 1) without performing any Proof of Work (PoW), *or* 2) some small amount (up to configuration of the pool), reflecting that no (quite extensive in mobile terms) BitMessage PoW was required to be performed. 
In the event that the person with which they wish to speak is not located in that pool, the message should be routed out of the pool and into the wider BitMessage network, arriving at either a standard BitMessage address, or at another pool.

A user should receive their messages from a pool just like how BitMessage itself works (prospective decryption), but in a much smaller scale. The connection between the user and the pool should be encrypted (although not strictly necessary even), but most probably not with SSL. Most probably the encryption will be derived from a token the user receives when first setting up with the pool (symmetric cipher), or some asymmetric scheme (like 1-Pass Unified Model EC-DHC).

** *Note:* These details are still being worked out currently, but that is the forecast. If you have any suggestions for improvements or corrections, please do.**

