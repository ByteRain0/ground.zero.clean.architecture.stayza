# ground.zero.clean.architecture.stayza



Avantajele unui singur agregat root Book pentru toate:

    Consistență tranzacțională: Toate modificările asupra copiei, împrumuturilor și rezervărilor se fac într-o singură tranzacție. Nu ai probleme cu sincronizarea datelor.

    Simplificare la nivel de model: Ai un singur punct de intrare pentru toate operațiile legate de carte.

    Ușor de înțeles pentru cei care nu cunosc detaliile: „Cartea” este entitatea principală, iar restul sunt detalii interne.

Dezavantaje și riscuri:

    Agregatul poate deveni prea mare și complex (anemic vs big ball of mud). Dacă Book gestionează sute de copii, împrumuturi și rezervări, acesta poate deveni greu de menținut și performanța poate avea de suferit.

    Blocaje de concurență: Pentru că totul e într-un singur agregat, orice modificare a oricărei copii sau rezervări blochează întreg agregatul, ceea ce poate duce la blocaje în aplicații cu încărcare mare.

    Scalabilitate redusă: Nu poți scala separat logica pentru împrumuturi sau rezervări. Totul merge împreună.