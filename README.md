# Dungeon Knight Adventure
3D Roguelike 迷宮探索遊戲  

##
迷宮隨機生成
![image](3DDungeonGenerate.gif) 
1. 在一定空間內產生一些大小不一的房間

2. 以Delaunay triangulation做出連接所有房間的Graph，然後產生Minimum Spanning Tree作為基本路線

3. 將一部分在MST生成中被剔除的冗餘Edge放回路線，使迷宮更複雜

4. 配合A*演算法，生成路線上的地板和階梯

##
能力值與裝備
![image](Equipments.jpg) 

##
[Demo Video](https://youtu.be/Dnx5DR5pg6o)

[Build](https://drive.google.com/file/d/1a9_40h4c44H7zcufbKbu9T_Cf03BoxWh/view?usp=drive_link/)
