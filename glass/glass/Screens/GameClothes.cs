﻿/*
 * Created by SharpDevelop.
 * User: ds930619
 * Date: 2010-11-22
 * Time: 10:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using glass.framework;
using glass.config;

namespace glass.Screens
{
	/// <summary>
	/// Description of GameClothes.
	/// </summary>
	/// 
	
	partial class GameClothes : Form {
		private Dictionary<Difficulty, string[]> ClothSounds = new Dictionary<Difficulty, string[]> {
			{Difficulty.easy, new string[]{"troja.wav","byxor.wav","mossa.wav"}},
			{Difficulty.normal, new string[]{"troja.wav","strumpor.wav","byxor.wav","mossa.wav","jacka.wav"}},
			{Difficulty.hard, new string[]{"t-shirt.wav","troja.wav","strumpor.wav","byxor.wav","mossa.wav","jacka.wav"}},
		};
		private Dictionary<Difficulty, int[]> Clothes = new Dictionary<Difficulty, int[]> {
			{Difficulty.easy, new int[]{0,1,3}},
			{Difficulty.normal, new int[]{0,2,1,3,0}},
			{Difficulty.hard, new int[]{0,0,2,1,3,0}},
		};
		int ActiveClothes=0;
		struct ClothPlaces {
			private string name;
			private Rectangle poly;
			public string Name {
				get{return(name);}
				set{name=value;}
			}
			public Rectangle Polygon {
				get{return(poly);}
				set{poly=value;}
			}
			public ClothPlaces(string n, Rectangle p) {
				name=n;
				poly=p;
			}
		};
		
		private ClothPlaces goal;
		//private Bitmap current;
		
		Bitmap[] trojor=new Bitmap[] {
			global::glass.Resources.troja_bla,
			global::glass.Resources.troja_rod,
			global::glass.Resources.troja_gron,
			global::glass.Resources.troja_gul,
		};
		Bitmap[] byxor=new Bitmap[] {
			global::glass.Resources.byxa_bla,
			global::glass.Resources.byxa_rod,
			global::glass.Resources.byxa_gron,
			global::glass.Resources.byxa_gul,
		};
		Bitmap[] mossor=new Bitmap[] {
			global::glass.Resources.mossa_bla,
			global::glass.Resources.mossa_rod,
			global::glass.Resources.mossa_gron,
			global::glass.Resources.mossa_gul,
		};
		Bitmap[] strumpor=new Bitmap[] {
			global::glass.Resources.strumpa_bla,
			global::glass.Resources.strumpa_rod,
			global::glass.Resources.strumpa_gron,
			global::glass.Resources.strumpa_gul,
		};
		Bitmap[] tshirts=new Bitmap[] {
			global::glass.Resources.t_bla,
			global::glass.Resources.t_rod,
			global::glass.Resources.t_gron,
			global::glass.Resources.t_gul,
		};
		Bitmap[] jackor=new Bitmap[] {
			global::glass.Resources.jacka_bla,
			global::glass.Resources.jacka_rod,
			global::glass.Resources.jacka_gron,
			global::glass.Resources.jacka_gul,
		};
		bool dragging;
		Point offset;
		ClothPlaces[] PlaceCollection = new ClothPlaces[]{
			new ClothPlaces("troja",new Rectangle(268,117, 100,100)),
			new ClothPlaces("byxor",new Rectangle(278,298, 100,80)),
			new ClothPlaces("strumpor",new Rectangle(251,436, 200, 60)),
			new ClothPlaces("mossa",new Rectangle(336,10, 50, 60)),
		};
		public GameClothes() {
			InitializeComponent();
			DrawableItems d=new DrawableItems();
			d.Parent=drawArea1;
			d.Image=global::glass.Resources.gubbe;
			d.Bounds=new Rectangle(250,20,300,500);
			drawArea1.Items.Add(d);
			
			SpawnClothes();
		}
		
		void PicBackClick(object sender, EventArgs e) {
			Dialog.YesNoDialog AreYouSure =new glass.Dialog.YesNoDialog();
			AreYouSure.ShowDialog();
			if(AreYouSure.Answer)
				this.Close();
		}
		private void ClickMovable(DrawableItems sender, MouseEventArgs e) {
			drawArea1.ActiveItem=sender;
			offset=new Point(e.X,e.Y);
			int x=(int)((sender.Bounds.X+sender.Bounds.X+sender.Bounds.Width)/2);
			int y=(int)((sender.Bounds.Y+sender.Bounds.Y+sender.Bounds.Height)/2);
			dragging=(!dragging)&sender.Enabled;
			if((!dragging)&&(sender.Enabled)) {
				//if(sender.Tag==ActiveClothes&&goal.Polygon.Contains(new Point(x,y))) {
				if(sender.Tag==ActiveClothes&&goal.Polygon.Contains(sender.Bounds.Location)) {
					sender.Bounds=new Rectangle(goal.Polygon.Location.X+16,goal.Polygon.Y+16, sender.Bounds.Width, sender.Bounds.Height);
					sender.Enabled=false;
					drawArea1.Invalidate();
					ActiveClothes++;
					if(ActiveClothes==Clothes[Config.LoggedInUser.difficulty].Length) {
						if((Config.LoggedInUser.score&((int)Framework.LevelScores.Clothes<<(int)Config.LoggedInUser.difficulty))!=((int)Framework.LevelScores.Clothes<<(int)Config.LoggedInUser.difficulty)) {
					   		Config.LoggedInUser.score+=(uint)Framework.LevelScores.Clothes<<(int)Config.LoggedInUser.difficulty;
					   		Config.UpdateScore(Config.LoggedInUser);
					   	}
						Framework.sndPlay.SoundLocation=@"Sounds\bra.wav";
						Framework.sndPlay.Play();
						//MessageBox.Show("omglol");
						this.Close();
					}else{
						goal=PlaceCollection[Clothes[Config.LoggedInUser.difficulty][ActiveClothes]];
						Framework.sndPlay.SoundLocation=@"Sounds\Clothes\klapa.wav";
						Framework.sndPlay.PlaySync();
						Framework.sndPlay.SoundLocation=@"Sounds\Clothes\"+ClothSounds[Config.LoggedInUser.difficulty][ActiveClothes];
						Framework.sndPlay.Play();
						//MessageBox.Show(goal.Name+" "+goal.Polygon.X.ToString()+" "+goal.Polygon.Y.ToString());
						//picBack.Bounds=goal.Polygon;
						//drawArea1.Controls.Add(picBack);
					}
				}
			}
			if(sender.Enabled) {
				sender.Parent.BringItemToFront(sender);
			}
		}
		private void MoveMovable(DrawableItems sender,MouseEventArgs e) {
			if(dragging&&sender.Enabled) {
				Point p=sender.Bounds.Location;
				sender.Bounds=new Rectangle(new Point(p.X+e.X-offset.X,p.Y+e.Y-offset.Y),new Size(sender.Bounds.Width,sender.Bounds.Height));
				Rectangle InvalidateRect=new Rectangle(Math.Min(sender.Bounds.X,p.X),Math.Min(sender.Bounds.Y,p.Y),sender.Bounds.Width+Math.Abs(sender.Bounds.X-p.X),sender.Bounds.Height+Math.Abs(sender.Bounds.Y-p.Y));
				drawArea1.Invalidate(InvalidateRect);
				this.Cursor=Cursors.SizeAll;
			} else {
				this.Cursor=Cursors.Default;
			}
		}
		void SpawnClothes() {
			int l;
			if(Config.LoggedInUser.difficulty==Difficulty.easy) {
				DrawableItems d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=trojor[l];
				d.Tag=0;
				d.Bounds=new Rectangle(0,0,236,185);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=byxor[l];
				d.Tag=1;
				d.Bounds=new Rectangle(600,100,213,178);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=mossor[l];
				d.Tag=2;
				d.Bounds=new Rectangle(600,300,87,60);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
			}else if(Config.LoggedInUser.difficulty==Difficulty.normal) {
				DrawableItems d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=trojor[l];
				d.Tag=0;
				d.Bounds=new Rectangle(0,0,236,185);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=strumpor[l];
				d.Tag=1;
				d.Bounds=new Rectangle(200,510,274,38);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=byxor[l];
				d.Tag=2;
				d.Bounds=new Rectangle(550,100,213,178);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=mossor[l];
				d.Tag=3;
				d.Bounds=new Rectangle(20,300,87,60);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=jackor[l];
				d.Tag=4;
				d.Bounds=new Rectangle(600,300,237,187);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
			}else if(Config.LoggedInUser.difficulty==Difficulty.hard) {
				DrawableItems d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=tshirts[l];
				d.Tag=0;
				d.Bounds=new Rectangle(0,0,236,180);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=trojor[l];
				d.Tag=1;
				d.Bounds=new Rectangle(500,50,236,185);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=strumpor[l];
				d.Tag=2;
				d.Bounds=new Rectangle(200,510,274,38);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=byxor[l];
				d.Tag=3;
				d.Bounds=new Rectangle(600,200,213,178);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=mossor[l];
				d.Tag=4;
				d.Bounds=new Rectangle(20,200,87,60);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
				
				
				d=new DrawableItems();
				d.Parent=drawArea1;
				l=Framework.rndInt(0,trojor.Length);
				d.Image=jackor[l];
				d.Tag=5;
				d.Bounds=new Rectangle(600,400,237,187);
				d.MouseDown+= new DrawableItems.ItemMouseEventHandler(ClickMovable);
				d.MouseMove+=new DrawableItems.ItemMouseEventHandler(MoveMovable);
				drawArea1.Items.Add(d);
			} 
			goal=PlaceCollection[0];
			ActiveClothes=0;
			Framework.sndPlay.SoundLocation=@"Sounds\Clothes\klapa.wav";
			Framework.sndPlay.PlaySync();
			Framework.sndPlay.SoundLocation=@"Sounds\Clothes\"+ClothSounds[Config.LoggedInUser.difficulty][ActiveClothes];
			Framework.sndPlay.Play();
		}
	}
}
