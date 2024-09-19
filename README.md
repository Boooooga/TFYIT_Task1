# Первая лаба по теории формальных языков и трансляций
Поддерживаются детерминированный конечный автомат (ДКА) и недетерминированный конечный автомат (НКА). 
Скоро будет недетерминированный с эпсилон-переходами.  
### Формат ввода автомата:  
DKA/NKA/NKA-E #тип_автомата  
Q: [state1], [state2], [state3]... #состояния_автомата  
S: [symbol1], [symbol2], [symbol3]... #алфавит  
Q0: [init_state] #начальное_состояние  
F: [final_state1], [final_state2]... #финальные_состояния   
Table:  
таблица |Q|x|S|, на пересечении состояния и входного символа располагается состояние, в которое перейдёт автомат
### Пример для ДКА:
![2024-09-19 18_27_11-DKA txt – Блокнот](https://github.com/user-attachments/assets/c80408e5-854b-4ad1-86c3-069009f4533f)
### Пример для НКА:
![2024-09-19 18_27_46-NKA txt – Блокнот](https://github.com/user-attachments/assets/b1ea6294-c7e7-4991-9801-5421e8f305b4)
