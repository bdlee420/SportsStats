﻿delete from stattypes 
insert into stattypes(id, DisplayName, name, DefaultShow, IsCalculated, SelectionDisplayOrder, GridDisplayOrder, ValueType, SportID, QuickButtonOrder, QuickButtonText, IsPositive, AutoGenerated, ShowGame, ShowTeam, ShowPlayer)
-- id, DisplayName, name,				DefaultShow, IsCalculated, SelectionDisplayOrder, GridDisplayOrder, ValueType, SportID, QuickButtonOrder, QuickButtonText, IsPositive, AutoGenerated,   ShowGame,   ShowTeam,   ShowPlayer
select 1, 'PTS', 'Points',					1,			1,				0,						5,				1,		1,          null,				null,		null,			0,				1,			1,			1
UNION
select 2, 'REB', 'Rebounds',                1,			0,				5,						41,				1,		1,          5,					'REB',		1,				0,				1,			1,			1
UNION
select 3, 'STL', 'Steals',                  1,			0,				30,						51,				1,		1,          8,					'STL',		1,				0,				1,			1,			1
UNION
select 4, 'AST', 'Assists',                 1,			0,				35,						46,				1,		1,          6,					'A',		1,				0,				1,			1,			1
UNION
select 5, 'Pass', 'Passes',                 0,			0,				100,					99,				1,		1,          null,				null,		1,				0,				0,			0,			1
UNION
select 6, '2pt Made', '2pt Made',           0,			0,				10,						99,				1,		1,          1,					'2FG',		1,				0,				0,			0,			1
UNION
select 7, '2pt Miss', '2pt Missed',			0,			0,				15,						99,				1,		1,          2,					'2FG',		0,				0,				0,			0,			1
UNION
select 8, 'FTM', 'FTM',						1,			0,				50,						12,				1,		1,          null,				null,		null,			0,				1,			1,			1
UNION
select 9, 'FT Miss', 'Free Throws Missed',  0,			0,				55,						99,				1,		1,          null,				null,		null,			0,				0,			0,			1
UNION
select 10, 'PP', 'Periods Played',          0,			0,				100,					99,				1,		1,          null,				null,		null,			0,				0,			0,			1
UNION
select 11, 'Foul', 'Foul',                  0,			0,				100,					99,				1,		1,          null,				null,		null,			0,				0,			0,			1
UNION
select 12, 'FGA', 'Field Goals Attempted',  1,			1,				0,						7,				1,		1,          null,				null,		null,			0,				1,			1,			1
UNION
select 13, 'FG%', 'Field Goal %',           1,			1,				0,						8,				2,		1,          null,				null,		null,			0,				1,			1,			1
UNION
select 14, 'FT%', 'Free Throw %',           1,			1,				0,						14,				2,		1,          null,				null,		null,			0,				1,			1,			1
UNION
select 15, 'FTA', 'Free Throw Attempted',   1,			1,				0,						13,				1,		1,          null,				null,		null,			0,				1,			1,			1
UNION
-- id, DisplayName, name,				DefaultShow, IsCalculated, SelectionDisplayOrder, GridDisplayOrder, ValueType, SportID, QuickButtonOrder, QuickButtonText, IsPositive, AutoGenerated,   ShowGame,   ShowTeam,   ShowPlayer   
select 16, 'BLK', 'Blocks',					1,			0,				35,						56,				1,			1,      7,					'BLK',		1,				0,				1,			1,			1
UNION
select 17, 'Force', 'Force Turnover',		0,			0,				42,						99,				1,			1,      null,				null,		null,			0,				0,			0,			1
UNION
select 18, 'PPG', 'Points Per Game',		1,			1,				0,						3,				4,			1,      null,				null,		null,			1,				0,			1,			1
UNION
select 19, 'G', 'Games',					0,			0,				0,						1,				1,			1,      null,				null,		null,			0,				0,			1,			1
UNION
select 20, 'RPG', 'Rebounds Per Game',		1,			1,				0,						40,				4,			1,      null,				null,		null,			1,				0,			1,			1
UNION
select 21, 'APG', 'Assists Per Game',		1,			1,				0,						45,				4,			1,      null,				null,		null,			1,				0,			1,			1
UNION
select 22, 'SPG', 'Steals Per Game',		1,			1,				0,						50,				4,			1,      null,				null,		null,			1,				0,			1,			1
UNION
select 23, 'BPG', 'Blocks Per Game',		1,			1,				0,						55,				4,			1,      null,				null,		null,			1,				0,			1,			1
UNION
select 24, 'TPG', 'Turnovers Per Game',		1,			1,				0,						60,				4,			1,      null,				null,		null,			1,				0,			1,			1
UNION
select 31, '3PM', '3pt Made',				1,			0,				20,						9,				1,			1,      3,					'3FG',		1,				0,				1,			1,			1
UNION
select 32, '3pt Miss', '3pt Miss',			0,			0,				25,						99,				1,			1,      4,					'3FG',		0,				0,				0,			0,			1
UNION
select 33, '3P%', '3P%',					1,			1,				0,						11,				2,			1,      null,				null,		null,			0,				1,			1,			1
UNION
select 34, '3PA', '3PA',					1,			1,				0,						10,				1,			1,      null,				null,		null,			0,				1,			1,			1
UNION 
select 35, 'FGM', 'FGM',					1,			1,				0,						6,				1,			1,      null,				null,		null,			0,				1,			1,			1
UNION 
select 40, 'TO', 'Turnover',				1,			0,				36,						61,				1,			1,      7,					'TO',		1,				0,				1,			1,			1
UNION 
select 200, 'IsActive', 'IsActive',			0,			0,				0,						0,				1,			1,      null,				null,		1,				0,				0,			0,			0

insert into stattypes(id, DisplayName, name, DefaultShow, IsCalculated, SelectionDisplayOrder, GridDisplayOrder, ValueType, SportID, QuickButtonOrder, QuickButtonText, IsPositive, AutoGenerated)

select 50, 'AB', 'At Bats',                     0, 1, 0, 5, 1, 2, null, null, null, 0
UNION
select 51, 'H', 'Hits',							0, 1, 0, 10, 1, 2, null, null, null, 0
UNION
select 52, 'RBI', 'RBI',						0, 0, 50, 15, 1, 2, null, null, null, 1
UNION
select 53, 'R', 'Runs',							0, 0, 55, 20, 1, 2, null, null, null, 1
UNION
select 54, '1B', '1B',							0, 0, 30, 25, 1, 2, null, null, null, 0
UNION
select 55, '2B', '2B',							0, 0, 35, 30, 1, 2, null, null, null, 0
UNION
select 56, '3B', '3B',							0, 0, 40, 35, 1, 2, null, null, null, 0
UNION
select 57, 'HR', 'HR',							0, 0, 45, 40, 1, 2, null, null, null, 0
UNION
select 58, 'BB', 'BB',							0, 0, 60, 45, 1, 2, null, null, null, 0
UNION
select 59, 'SO', 'Strikeout',                   0, 0, 25, 50, 1, 2, null, null, null, 0
UNION
select 60, 'FC', 'Fielder''s Choice',           0, 0, 65, 55, 1, 2, null, null, null, 0
UNION
select 61, 'AVG', 'AVG',						0, 1, 0, 60, 3, 2, null, null, null, 0
UNION
select 62, 'OBP', 'OBP',						0, 1, 0, 65, 3, 2, null, null, null, 0
UNION
select 63, 'SLG', 'SLG',						0, 1, 0, 70, 3, 2, null, null, null, 0
UNION
select 64, 'GND OUT', 'Ground Out',				0, 0, 27, 75, 1, 2, null, null, null, 0
UNION
select 65, 'FLY OUT', 'Fly Out',				0, 0, 28, 80, 1, 2, null, null, null, 0
UNION
select 66, 'SWING MISS', 'Swing Miss',		    0, 0, 5, 85, 1, 2, null, null, null, 0
UNION
select 67, 'FOUL', 'FOUL',						0, 0, 20, 90, 1, 2, null, null, null, 0
UNION
select 68, 'BALL', 'Ball',						0, 0, 15, 95, 1, 2, null, null, null, 0
UNION
select 69, 'STRIKE LOOKING', 'Strike Looking',	0, 0, 85, 100, 1, 2, null, null, null, 0
UNION
select 70, 'SAC FLY', 'Sacrifice Fly',	        0, 0, 70, 105, 1, 2, null, null, null, 0
UNION
select 71, 'TB', 'Total Bases',	                0, 1, 75, 110, 1, 2, null, null, null, 0
UNION
select 72, 'Batting Order', 'Batting Order',	0, 0, 80, 115, 1, 2, null, null, null, 1
UNION
select 73, 'AVG RISP', 'AVG RISP',	0, 1, 0, 62, 3, 2, null, null, null, 0
UNION
select 74, 'AVG 2 OUTS', 'AVG 2 OUTS',	0, 1, 0, 71, 3, 2, null, null, null, 0
UNION
select 75, 'CONTACT', 'Contact',	0, 1, 0, 72, 2, 2, null, null, null, 0

insert into stattypes(id, DisplayName, name, DefaultShow, IsCalculated, SelectionDisplayOrder, GridDisplayOrder, ValueType, SportID, QuickButtonOrder, QuickButtonText, IsPositive, AutoGenerated)

select 100, 'Goals', 'Goals',                    0, 0, 5, 5, 1, 3, 1, 'G', 1, 0
UNION
select 101, 'Assists', 'Assists',				0, 0, 10, 10, 1, 3, 2, 'A', 1, 0
UNION
select 102, 'Points', 'Points',					0, 1, 0, 15, 1, 3, null, null, null, 0
UNION
select 103, 'PM', 'PM',							0, 0, 0, 20, 1, 3, 3, 'PM', 1, 0
UNION
select 104, 'Shootout Goals', 'Shootout Goals', 0, 0, 5, 25, 1, 3, 4, 'SG', 1, 0
UNION
select 105, 'Max Points', 'Max Points',           0,			1,				0,						200,				1,		1,          null, null, null, 0