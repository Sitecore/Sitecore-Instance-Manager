1
install a
delete &(website)\web.config
delete a
no a

2
install a
delete &(website)\app_config
delete a
no a

3-1
install-cm a
install-cm b ref:a
install-cd c ref:a
install-cd d ref:b
no-databases b
no-databases c
no-databases d
delete a

3-2
install-cm a
install-cm b ref:a
install-cd c ref:a
install-cd d ref:b
delete d
exist a
exist b
exist c
no d

3-3
install-cm a
install-cm b ref:a
install-cd c ref:a
install-cd d ref:b
delete c
exist a
exist b
no c
exist d

3-4
install-cm a
install-cm b ref:a
install-cd c ref:a
install-cd d ref:b
delete b
exist a
no b
exist c
no d

3-5
install-cm a
install-cm b ref:a
install-cd c ref:a
install-cd d ref:b
delete a
no a
no b
no c
no d