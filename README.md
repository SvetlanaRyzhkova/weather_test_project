﻿1. Погодное API
https://open-meteo.com/
Необходимо сделать консольную утилиту, выводящую прогноз погоды на экран (консоль). 
Можно и нужно использовать возможности Unicode.
На входе можно задавать интересующий город, и вид прогноза (количество дней вперед).
Предусмотреть кеширование результатов - использовать EF.
В качестве БД можно использовать SQLite.
Выдает справку.

Примеры
	-- weather.exe LONDON 3d
	-- выдает прогноз погоды в Лондоне на 3 дня вперед
	-- weather.exe -h
	-- weather.exe —help

Если погода берется из кеша, то необходимо это явно выводить пользователю.
При запросе погоды нужен явный ключ, который заставляет брать погоду с сервера, даже если в кеше есть значения.
Нужны unit тесты.
Нужны integration тесты.