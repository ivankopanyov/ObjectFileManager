# ObjectFileManager  
#### Итоговый проект по курсу "Введение в ООП"

## Десктопное приложение.

![Десктопное приложение ObjectFileManager](/image.jpg)

## Консольное приложение.
##### Список команд.
- [drives](#drives) - Список системных дисков.
- [cd](#cd) - Изменение текущей директории.
- [ls](#ls) - Список файлов и директорий из текущей директории.
- [file](#file) - Создание нового файла.
- [dir](#dir) - Создание новой директории.
- [copy](#copy) - Копирование файла или директории.
- [move](#move) - Перемещение файла или директории.
- [attr](#attr) - Изменение атрибутов файла или директории.
- [rm](#rm) - Удаление файла или директории.
- [info](#info) - Информация о файле или директории.
- [help](#help) - Список команд с описанием или описание команды с примерами использования.
- [exit](#exit) - Выход из приложения.

# drives
##### Список системных дисков.

```sh
C:\dir> drives  
```

# cd
##### Изменение текущей директории.

```sh
C:\dir> cd C:\dir_name\file_name
```
```sh
C:\dir> cd "..\dir_name"
```

# ls
##### Список файлов и директорий из текущей директории.

```sh
C:\dir> ls
```

# file
##### Создание нового файла.

```sh
C:\dir> file C:\dir_name\new_file_name
```
```sh
C:\dir> file "..\dir_name\new file name"
```

# dir
##### Создание новой директории.

```sh
C:\dir> dir C:\dir_name\new_dir_name
```
```sh
C:\dir> dir "..\dir_name\new dir name"
```

# copy
##### Копирование файла или директории.

```sh
C:\dir> copy C:\dir_name\file_name C:\dir_name\new_file_name
```
```sh
C:\dir> copy "..\dir name" "new dir name"
```
```sh
C:\dir> copy C:\dir_name\file_name "..\dir name\new_file_name"
```

# move
##### Перемещение файла или директории.

```sh
C:\dir> move C:\dir_name\file_name C:\dir_name\new_file_name
```
```sh
C:\dir> move "..\dir name" "new dir name"
```
```sh
C:\dir> move C:\dir_name\file_name "..\dir name\new_file_name"
```

# attr
##### Изменение атрибутов файла или директории.

```sh
C:\dir> attr hidden=true C:\dir_name\file_name
```
```sh
C:\dir> attr readonly=false hidden=true "..\dir_name"
```

# rm
##### Удаление файла или директории.

```sh
C:\dir> rm C:\dir_name\file_name
```
```sh
C:\dir> rm "..\dir_name"
```

# info
##### Информация о файле или директории.

```sh
C:\dir> info C:dir_name\file_name
```
```sh
C:\dir> info "..\dir_name"
```

# help
##### Список команд с описанием или описание команды с примерами использования.

```sh
C:\dir> help
```
```sh
C:\dir> help command_name
```

# exit
##### Выход из приложения.

```sh
C:\dir> exit
```