using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCup;
namespace TuringCupAI
{
    public class GameAI
    {
        const String teamname = "三L";
        int side = -1;
        double temp;
        int minindex, myindex;
        int i, j, k, p, q, u, v;
        bool hero_ok = false;       //标记英雄细胞可以独立行动了
        int my_hero = -1;
        bool my_first_step = true;
        bool[][] back_attack = new bool[600][];
        bool once = true;
        bool[] in_my_area = new bool[600];     //标记一个细胞是不是在我的领域内

        public void init(int side)
        {
            this.side = side;
        }
        public String main(String str)
        {
            GameState gstate = GameState.Load(str);
            AI_Commands commands = new AI_Commands();
            String result = "";
            List<int> mycell = new List<int>();
            for (i = 0; i < gstate.cells.Count; i++)
            {
                if (gstate.cells[i].side == side)
                    mycell.Add(i);
            }  
          
//------------------------------------------LightTime----------------------------------------------------

            
            
const int MAXN = 405;
const int LINKWEIGHT = 75;
const int HATEWEIGHT = 200;
const int DISWEIGHT = 20;
const double EPS = 0.5;
int[] num_of_tent = new int[5] { 1, 1, 2, 2, 3 };
double[][] dis = new double[MAXN][];	//记录每两个细胞之间的距离
int[] hero_support = new int[MAXN];       //我方主细胞只需要供应离自己最近的几个细胞。

int edge_s,edge_t;		//边的起点和终点细胞编号
int[][] Tents = new int[MAXN][];		//记录连接i，j细胞的触手的状态，值为0表示不存在，值为1表示正在伸，值为2表示已经连接上。
int[] cell_out = new int[MAXN];		    //记录每个细胞伸出的触手数
int[] my_cell_to_this = new int[MAXN];		//记录我方细胞连接到这个细胞的触手数
int[] enemy_cell_to_this = new int[MAXN];	//记录敌方细胞连接到这个细胞的触手数
int[] enemy_will_to_this = new int[MAXN];	//记录敌方细胞到这个细胞的正在伸的过程中的触手数
int[] linked_degree = new int[MAXN];		//my_cell_to_this - enemy_cell_to_this
int[] linking_degree = new int[MAXN];		//my_cell_to_this - enemy_cell_to_this - enemy_will_to_this
int[] e_2_t = new int[MAXN];
int[] m_2_t = new int[MAXN];
int[] cell_num = new int[MAXN];     //根据与英雄细胞的距离进行编号，用来构成完美环
int my_area_cell_num = 0;       //记录上一个数组的大小

for (i = 0; i < gstate.cells.Count; ++i)
{
    dis[i] = new double[MAXN];
    Tents[i] = new int[MAXN];	
    for (j = 0; j < gstate.cells.Count; ++j)
    {
        Tents[i][j] = 0;
        dis[i][j] = GameState.distance(gstate.cells[i], gstate.cells[j]);
    }
}

//求出我的区域，等一个实现
//in_my_area[1] = in_my_area[12] = in_my_area[13] = in_my_area[16] = in_my_area[17] = true;
if (once == true)
{
    once = false;
    for (i = 0; i < gstate.cells.Count; ++i)
    {
        back_attack[i] = new bool[MAXN];
        for (j = 0; j < gstate.cells.Count; ++j)
            back_attack[i][j] = false;
    }

    int cellnum = gstate.cells.Count;
    int ly_index_ = -1; 
    for (i = 0; i < cellnum; ++i)
        in_my_area[i] = false;

    for (i = 0; i < cellnum; i++)
        if (gstate.cells[i].side == side)
           ly_index_ = i;

    in_my_area[ly_index_] = true;
    int hero_2, hero_3, hero_4;
    int[] hero = new int[10];
    j = 0;
    for (i = 0; i < cellnum; ++i)
    {
        if (gstate.cells[i].ishero == true && i != ly_index_)
        {
            hero[j++] = i;
        }
    }
    hero_2 = hero[0];
    hero_3 = hero[1];
    hero_4 = hero[2];   //hero[]就没用了
    int[] f = new int[10];
    f[0] = ly_index_;
    f[1] = hero_2;
    f[2] = hero_3;
    f[3] = hero_4;
    double min1, min2;
    int index1, index2;
    double[] dis_hero = new double[10];
    for (k = 0; k < cellnum; ++k)
    {
        if (gstate.cells[k].side == 0)
        {
            dis_hero[0] = dis[k][ly_index_];
            dis_hero[1] = dis[k][hero_2];
            dis_hero[2] = dis[k][hero_3];
            dis_hero[3] = dis[k][hero_4];
        }
        else
        {
            continue;
        }
        min1 = dis_hero[0];
        index1 = 0;
        for (i = 1; i < 4; ++i)
        {
            if (dis_hero[i] < min1)
            {
                min1 = dis_hero[i];
                index1 = i;
            }
        }
        min2 = 99999;
        index2 = 99999;
        for (i = 0; i < 4; ++i)
        {
            if (dis_hero[i] < min2 && i != index1)
            {
                min2 = dis_hero[i];
                index2 = i;
            }
        }
        if ((index1 == 0 || index2 == 0) && min1 == min2)
        {
            //my_border[my_border_cell_num++] = k;
            in_my_area[k] = true;
        }
        if (index1 == 0 && min1 != min2)
        {
            //cells_in_my_area[my_cell_num++] = k;
            in_my_area[k] = true;
        }
    }
}

//求每两个细胞之间的距离，以及初始化各个数组
for (i = 0; i < gstate.cells.Count; ++i)	
{
    if (in_my_area[i] == true)
    {
        cell_num[my_area_cell_num] = i;
        ++my_area_cell_num;
    }
    if (gstate.cells[i].ishero == true && gstate.cells[i].side == side)
        my_hero = i;

}

//根据距离我的英雄细胞的远近求出cell_num
for (p = 0; p < my_area_cell_num; ++p)
{
    u = cell_num[p];
    for (q = p + 1; q < my_area_cell_num; ++q)
    {
        v = cell_num[q];
        if (dis[v][my_hero] < dis[u][my_hero])
        {
            int ttm = cell_num[p];
            cell_num[p] = cell_num[q];
            cell_num[q] = ttm;
            u = cell_num[p];
        }
    }
}


for (i = 0; i < gstate.tents.Count; ++i)	//触手信息
{
	edge_s = gstate.tents[i].begin;
	++cell_out[edge_s];		

	edge_t = gstate.tents[i].end;
	Tents[edge_s][edge_t] = 1;

    if (gstate.tents[i].len1 > 5)
    {
        if (gstate.cells[edge_s].side == side)
            ++m_2_t[edge_t];
        else
            ++e_2_t[edge_t];
    }

	double tent_tmp = gstate.tents[i].len1 - dis[edge_s][edge_t];
	if (tent_tmp < 0)
		tent_tmp *= -1;
	if (tent_tmp < EPS)		//表示已经连接上了，不是在伸的过程中
	{
		++Tents[edge_s][edge_t];
		if (gstate.cells[edge_s].side == side)
			++my_cell_to_this[edge_t];
		else
			++enemy_cell_to_this[edge_t];
	}
	else
	{
		if (gstate.cells[edge_s].side != side)
		{
			++enemy_will_to_this[edge_t];
		}
	}
}
for (i = 0; i < gstate.cells.Count; ++i)
{
	linked_degree[i] = my_cell_to_this[i] - enemy_cell_to_this[i];
	linking_degree[i] = my_cell_to_this[i] - enemy_cell_to_this[i] - enemy_will_to_this[i];
}


//更新back_attack的bool数组
for (i = 0; i < gstate.cells.Count; ++i)
{
    for (j = 0; j < gstate.cells.Count; ++j)
    {
        if (back_attack[i][j] == true)
            commands.ntents.Add(new AI_Commands.NewTentacle(i, j));
    }
}
for (i = 0; i < gstate.cells.Count; ++i)
{
    for (j = 0; j < gstate.cells.Count; ++j)
    {
        if (Tents[i][j] > 0)
            back_attack[i][j] = false;
    }
}       

//------------------------------------------特别行动之——英雄细胞行动----------------------------------------
u = my_hero;
v = cell_num[1];
commands.ntents.Add(new AI_Commands.NewTentacle(u, v));
v = cell_num[2];
if (Tents[v][u] == 2 && gstate.cells[my_hero].hp > 60)
{
    commands.btents.Add(new AI_Commands.BreakTentacle(v, u, 999));
    commands.ntents.Add(new AI_Commands.NewTentacle(u, v));
}
if (Tents[v][u] != 1)          
    commands.ntents.Add(new AI_Commands.NewTentacle(u, v));
v = cell_num[3];
if (Tents[v][u] == 2 && gstate.cells[my_hero].hp > 120)
{
    commands.btents.Add(new AI_Commands.BreakTentacle(v, u, 999));
    commands.ntents.Add(new AI_Commands.NewTentacle(u, v));
}
if (Tents[v][u] != 1)  
    commands.ntents.Add(new AI_Commands.NewTentacle(u, v));


//--------------------------------------------英雄细胞行动结束----------------------------------------------


//------------------------------------------0级行动，把多余的支援触手割掉------------------------------------

int[] cut_queue = new int[MAXN];	//本级行动中要切掉的触手
int cut_number = 0;		//本级行动中要切掉的触手总数
for (i = 0; i < gstate.cells.Count; ++i)
{
	cut_number = 0;
	//割己方触手的规则1：细胞血量必须达到149；如果是英雄细胞，保证其degree>=0，如果不是，保证其degree>=1
	if (gstate.cells[i].side != side || gstate.cells[i].hp < 149 || (gstate.cells[i].ishero == true && linking_degree[i] <= 1) || (gstate.cells[i].ishero == false && linking_degree[i] <= 2))
		continue;

	//先把所有正在伸的触手缩回去
	for (j = 0; j < gstate.cells.Count; ++j)
	{
		if (gstate.cells[j].side == side && Tents[j][i] == 1 && gstate.cells[j].ishero == false)
		{
			commands.btents.Add(new AI_Commands.BreakTentacle(j, i, 999));
            Tents[j][i] = 0;
		}
	}

	//求出所有已经与之连接上的触手数
	for (j = 0; j < gstate.cells.Count; ++j)
	{
		if (gstate.cells[j].side == side && Tents[j][i] == 2 && gstate.cells[j].ishero == false)
		{
			cut_queue[cut_number++] = j;
		}
	}

	int zero_tmp = 0;
	for (p = 0; p < cut_number; ++p)
	{
		double valuep = linking_degree[cut_queue[p]] * 100 + gstate.cells[cut_queue[p]].hp + dis[i][cut_queue[p]]*0.0707;
		for (q = p + 1; q < cut_number; ++q)
		{
			double valueq = linking_degree[cut_queue[q]] * 100 + gstate.cells[cut_queue[q]].hp + dis[i][cut_queue[q]]*0.0707;
			if (valueq > valuep)
			{
				zero_tmp = cut_queue[p];
				cut_queue[p] = cut_queue[q];
				cut_queue[q] = zero_tmp;
				valuep = linking_degree[cut_queue[p]] * 100 + gstate.cells[cut_queue[p]].hp + dis[i][cut_queue[p]]*0.0707;
			}
		}
	}
	zero_tmp = 0;
	int cut_end = 1;
	if (gstate.cells[i].ishero == true)
		cut_end = 0;
	while (linking_degree[i] >= cut_end)
	{
		commands.btents.Add(new AI_Commands.BreakTentacle(cut_queue[zero_tmp++], i, 999));
		--linking_degree[i];
        Tents[zero_tmp - 1][i] = 0;
	}
}
//------------------------------------------0级行动结束------------------------------------------------------



//---------------------------1.5级行动，如果一条触手起点终点都是我的，但是现在终点比起点情况好，那么反过来支援-------------
for (i = 0; i < gstate.tents.Count; ++i)
{
	edge_s = gstate.tents[i].begin;
	edge_t = gstate.tents[i].end;
    if (Tents[edge_s][edge_t] < 1)
        continue;
	if (gstate.cells[edge_s].side == side && gstate.cells[edge_t].side == side && gstate.cells[edge_s].level+1 < gstate.cells[edge_t].level && linked_degree[edge_s] < linked_degree[edge_t] && linked_degree[edge_t] >= 1 && cell_out[edge_t] < num_of_tent[gstate.cells[edge_t].level] && gstate.cells[edge_t].ishero == false && Tents[edge_s][edge_t] == 2)
	{
		commands.btents.Add(new AI_Commands.BreakTentacle(edge_s, edge_t, 999));
		commands.ntents.Add(new AI_Commands.NewTentacle(edge_t, edge_s));
        ++cell_out[edge_t];
        --cell_out[edge_s];
        Tents[edge_t][edge_s] = 1;
	}
}
//-------------------------------------------1.5级结束----------------------------------------------------


//----------------------------------------1.75级行动：对于纯白细胞，割边攻击----------------------------
for (i = 0; i < gstate.cells.Count; ++i)
{
    if (gstate.cells[i].occupy_side == side && gstate.cells[i].side == 0)
    {
        double sum = 0;
        for (j = 0; j < gstate.tents.Count; ++j)
        {
            if (gstate.cells[gstate.tents[j].begin].side == side)
            {
                if (gstate.tents[j].end == i && gstate.tents[j].len1 + 1.0 > GameState.distance(gstate.cells[gstate.tents[j].begin], gstate.cells[gstate.tents[j].end]))
                    sum += gstate.tents[j].len1 * 0.0707;
            }
        }
        if (gstate.cells[i].occupy_progress + sum >= gstate.cells[i].occupy_goal)
        {
            for (j = 0; j < gstate.tents.Count; ++j)
            {
                u = gstate.tents[j].begin;
                v = gstate.tents[j].end;
                if (gstate.tents[j].end == i && gstate.cells[gstate.tents[j].end].side == 0 && gstate.cells[gstate.tents[j].begin].side == side && gstate.tents[j].len1 + 1 > dis[gstate.tents[j].begin][gstate.tents[j].end] && Tents[u][v] == 2)
                    commands.btents.Add(new AI_Commands.BreakTentacle(gstate.tents[j].begin, gstate.tents[j].end, 0));
            }
        }
    }
}
//-----------------------------------------1.75级行动结束---------------------------------------------



//----------------------------------------------2级行动，打纯白--------------------------------------------------
bool[] visit = new bool[MAXN];		//标记一个细胞的靶细胞
int[] attack_queue = new int[MAXN];
int attack_number = 0;
for (i = 0; i < gstate.cells.Count; ++i)		//触手的起点：i细胞，终点：j细胞
{
    for (j = 0; j < gstate.tents.Count; ++j)
    {
        u = gstate.tents[j].begin;
        v = gstate.tents[j].end;
        if (gstate.cells[u].side == side)
            visit[v] = true;
    }
	if (gstate.cells[i].side != side)		//如果不是我方细胞，跳过
		continue;	
	for (j = 0; j < gstate.cells.Count; ++j)	//每个细胞初始化为没有访问
		visit[j] = false;
    attack_number = 0;
    for (j = 0; j < gstate.cells.Count; ++j)	//触手的终点：j
    {
        if (in_my_area[j] == false)
            continue;

        if (gstate.cells[j].side != 0 || visit[j] == true || enemy_cell_to_this[j] != 0 || gstate.cells[j].ishero == true)
            continue;

        if (my_cell_to_this[j] > 0)
            continue;

        attack_queue[attack_number++] = j;
    }
    for (p = 0; p < attack_number; ++p)
    {
        int third_tmp = 0;
        for (q = p + 1; q < attack_number; ++q)
        {
            if (dis[i][attack_queue[q]] < dis[i][attack_queue[p]])
            {
                third_tmp = attack_queue[p];
                attack_queue[p] = attack_queue[q];
                attack_queue[q] = third_tmp;
            }
        }
    }
    int third_end = attack_number;
    if (num_of_tent[gstate.cells[i].level] < third_end)
        third_end = num_of_tent[gstate.cells[i].level];
    for (k = 0; k < third_end; ++k)	    //触手的终点：attack_queue[k]
    {
        commands.ntents.Add(new AI_Commands.NewTentacle(i, attack_queue[k]));
        visit[attack_queue[k]] = true;
    }
}
//--------------------------------------------------2级结束--------------------------------------------------------


//---------------------------------------2.5级行动，初始情况下的构环--------------------------------------------
//在刚开始，根据完美环来构环
if (my_first_step == true)
{
    //如果有一个细胞有三个触手，结束该阶段，以后再也不进来了
    for (i = 0; i < gstate.cells.Count; ++i)
    {
        if (gstate.cells[i].side == side && num_of_tent[gstate.cells[i].level] > 2)
            my_first_step = false;
    }
    for (i = my_area_cell_num-1; i >= 0; --i)
    {
        u = cell_num[i];
        if (gstate.cells[u].side != side)
            continue;
        for (j = i - 1; j != i; )
        {
            if (j < 0)
                j = my_area_cell_num - 1;
            v = cell_num[j];


            if (gstate.cells[v].side != side)
            {
                --j;
                if (j < 0)
                    j = my_area_cell_num - 1;
                continue;
            }

            if (Tents[u][v] == 0 && Tents[v][u] == 0)
            {
                commands.ntents.Add(new AI_Commands.NewTentacle(u, v));  
            }
            --j;
            if (j < 0)
                j = my_area_cell_num - 1;
        }
    }
}
//--------------------------------------------------2.5级结束--------------------------------------------------------



//---------------------------------------------3级行动，支援-----------------------------------------	---------
for (i = 0; i < gstate.cells.Count; ++i)
{
    if (gstate.cells[i].side != side)
        continue;
    int[] support_queue = new int[MAXN];
    int support_num = 0;
    for (j = 0; j < gstate.cells.Count; ++j)	//如果有友方细胞级别比我低1以上并且支援触手数比我少，或者两者血量较低（说明处于初级阶段），去支援它
    {
        if (gstate.cells[j].ishero == true && (i == hero_support[0] || i == hero_support[1] || i == hero_support[2]))
            continue;
        if ((gstate.cells[j].level < 4 && Tents[i][j] == 0 && Tents[j][i] == 0 && gstate.cells[j].side == side) || (gstate.cells[j].side == side && gstate.cells[j].level < gstate.cells[i].level-1 && my_cell_to_this[j] <= enemy_cell_to_this[j] && Tents[i][j] == 0 && Tents[j][i] == 0) )
        {
            support_queue[support_num++] = j;
        }
    }
    //排序
    for (p = 0; p < support_num; ++p)
    {
        int tt_tmp = 0;
        u = support_queue[p];
        double tt_value1 = gstate.cells[u].hp + linked_degree[u] * 75;
        for (q = p + 1; q < support_num; ++q)
        {
            v = support_queue[q];
            double tt_value2 = gstate.cells[v].hp + linked_degree[v] * 75;
            if (tt_value2 < tt_value1)
            {
                tt_tmp = support_queue[p];
                support_queue[p] = support_queue[q];
                support_queue[q] = tt_tmp;
                tt_value1 = gstate.cells[u].hp + linked_degree[u] * 75;
            }
        }
    }
    int tt_end = support_num;
    for (k = 0; k < tt_end; ++k)
    {
        v = support_queue[k];
        commands.ntents.Add(new AI_Commands.NewTentacle(i, v));
        Tents[i][v] = 1;
    }
}
//---------------------------------------------3级行动结束------------------------------------------------------

//-----------------------------------------1级行动：反击----------------------------------------------------------
for (i = 0; i < gstate.tents.Count; ++i)	//遍历每条触手，如果有敌方细胞打我，立即反击
{
    edge_s = gstate.tents[i].begin;		//敌方的，攻击该细胞的细胞
    edge_t = gstate.tents[i].end;		//我方的，被攻击的细胞

    if (Tents[edge_s][edge_t] < 2)      //只处理已经打到我细胞的触手，不处理正在伸的过程中的
        continue;
    if (Tents[edge_t][edge_s] > 0)      //如果我已经伸出触手了，就不用再管了
        continue;
    if (back_attack[edge_t][edge_s] == true)
        continue;
    if (gstate.cells[edge_s].ishero == true)
        continue;

    if (gstate.cells[edge_s].side != side && gstate.cells[edge_t].side == side)
    {
        //如果该细胞有多余的触手，直接顶回去
        if (cell_out[edge_t] < num_of_tent[gstate.cells[edge_t].level])
        {
            back_attack[edge_t][edge_s] = true;
            commands.ntents.Add(new AI_Commands.NewTentacle(edge_t, edge_s));
            Tents[edge_t][edge_s] = 1;
            ++cell_out[edge_t];
        }
        //如果该细胞正在支援英雄细胞，把这条触手收回，顶回去
        else if (Tents[u][my_hero] > 0)
        {
            back_attack[edge_t][edge_s] = true;
            commands.btents.Add(new AI_Commands.BreakTentacle(edge_t, my_hero, 999));
            commands.ntents.Add(new AI_Commands.NewTentacle(edge_t, edge_s));
            continue;
        }
        //如果该细胞没有多余的触手，收回一条最次要的触手，顶回去
        else
        {
            cut_number = 0;
            u = edge_t;
            //从该细胞正在攻击的，且没有对攻回来的细胞中挑一个最难打下来的
            for (j = 0; j < gstate.cells.Count; ++j)
            {
                v = j;
                if (Tents[u][v] > 0 && gstate.cells[v].side != side && Tents[v][u] == 0)
                    cut_queue[cut_number++] = v;
            }
            int one_mx = 0;
            for (p = 0; p < cut_number; ++p)
            {
                v = cut_queue[one_mx];
                double one_value1 = dis[u][v] * 0.0707 + gstate.cells[v].hp - linked_degree[v] * 100;
                v = cut_queue[p];
                double one_value2 = dis[u][v] * 0.0707 + gstate.cells[v].hp - linked_degree[v] * 100;
                if (one_value2 > one_value1)
                    one_mx = p;
            }
            if (cut_number > 0)
            {
                back_attack[edge_t][edge_s] = true;
                commands.btents.Add(new AI_Commands.BreakTentacle(edge_t, cut_queue[one_mx], 999));
                commands.ntents.Add(new AI_Commands.NewTentacle(edge_t, edge_s));
                continue;
            }
        }
    }
}
//--------------------------------------------1级行动结束----------------------------------------------------------




//---------------------------------------------5级行动，攻击！-----------------------------------------
int[] hate = new int[MAXN];         //仇恨值，该细胞有一条伸向我的触手，则仇恨值加1 
//求仇恨值
for (i = 0; i < gstate.cells.Count; ++i)
    hate[i] = 0;
for (i = 0; i < gstate.tents.Count; ++i)
{
    u = gstate.tents[i].begin;
    v = gstate.tents[i].end;
    if (gstate.cells[u].side == side || gstate.cells[v].side != side)
        continue;
    ++hate[u];
}


for (i = 0; i < gstate.cells.Count; ++i)
{
    if (gstate.cells[i].side != side)
        continue;
    attack_number = 0;
    for (j = 0; j < gstate.cells.Count; ++j)
    {
        if (gstate.cells[j].side == side || gstate.cells[j].side == 0 || gstate.cells[j].ishero == true)
            continue;
        if (gstate.cells[i].hp - 40 < dis[i][j] * 0.0707)
            continue;
        attack_queue[attack_number++] = j;
    }

    for (p = 0; p < attack_number; ++p)		//对攻击队列进行排序
    {
        u = attack_queue[p];
        double value1 = gstate.cells[u].hp + dis[i][u] * 0.0707 * DISWEIGHT - linked_degree[u] * LINKWEIGHT - hate[u] * HATEWEIGHT;
        for (q = p + 1; q < attack_number; ++q)
        {
            v = attack_queue[q];
            double value2 = gstate.cells[v].hp + dis[i][v] * 0.0707 * DISWEIGHT - linked_degree[v] * LINKWEIGHT - hate[v] * HATEWEIGHT;
            if (value2 < value1)
            {
                int third_tmp = attack_queue[p];
                attack_queue[p] = attack_queue[q];
                attack_queue[q] = third_tmp;
                u = attack_queue[p];
                value1 = gstate.cells[u].hp + dis[i][u] * 0.0707 * DISWEIGHT - linked_degree[u] * LINKWEIGHT - hate[u] * HATEWEIGHT;
            }
        }
    }
    
    int five_end = attack_number;
    if (num_of_tent[gstate.cells[i].level] < five_end)
        five_end = num_of_tent[gstate.cells[i].level];
    if (attack_number > 0)
    {
        for (k = 0; k < five_end; ++k)	    //触手的终点：attack_queue[k]
            commands.ntents.Add(new AI_Commands.NewTentacle(i, attack_queue[k]));
    }
}
//--------------------------------------------5级行动结束-------------------------------------------------



//--------------------------------------------6级行动，保底处理，不要有触手不伸----------------------------------------------------
for (i = 0; i < gstate.cells.Count; ++i)
{
    if (gstate.cells[i].side != side)
        continue;
    for (j = 0; j < gstate.cells.Count; ++j)
    {
        if (j == i || (gstate.cells[j].ishero == true && gstate.cells[j].side != side))
            continue;
        if (i == 12 && j == 1)
            v = 0;
        commands.ntents.Add(new AI_Commands.NewTentacle(i, j));
    }
}
//--------------------------------------------6级行动结束-------------------------------------------------



//--------------------------------------------end--------------------------------------------------------
            result = commands.Serialize();
            return result;
        }
    }
}
