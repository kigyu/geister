using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostsScript : MonoBehaviour {

  private GameObject g_obj_;
  private Index2 p_idx_;
  private List<GhostCommonControl> p_hunted_ = new List<GhostCommonControl>();
  private GameObject[] pghosts = new GameObject[8];
  private GameObject[] eghosts = new GameObject[8];
  private GameObject[,] omap = new GameObject[8,8];
  private Vector3[,] vmap = new Vector3[8,8];
  private Action act;

  public struct Index2 {
    public int r;
    public int c;

    public Index2(int r_, int c_) {
      r = r_;
      c = c_;
    }
  }

  static public Vector3 Index2Pos(int r, int c) {
    var x = -2.5f + 1.0f * c;
    var y = +2.5f - 1.0f * r;
    return new Vector3(x, y, 0.0f);
  }

  static public Vector3 Index2Pos(Index2 idx) {
    return Index2Pos(idx.r, idx.c);
  }

  static public Index2 Pos2Index(float x, float y) {
    var c = +(int)Math.Round(x + 2.5f);
                var r = -(int)Math.Round(y - 2.5f);
    return new Index2(r, c);
  }

  static public Index2 Pos2Index(ref Vector3 pos) {
    //var c = +(int)Math.Round(pos.x + 2.5f);
                //var r = -(int)Math.Round(pos.y - 2.5f);
    return Pos2Index(pos.x, pos.y);
  }

  public GhostsScript() {
    for (int i = 0; i < 8; ++i) {
      for (int j = 0; j < 8; ++j) {
        vmap[i,j] = Index2Pos(i, j);
      }
    }

    //act = SetupOperation;
    act = PlayerOperation;
  }

  // Use this for initialization
  void Start () {
    // Initialize Player Ghosts
    GameObject prefab = (GameObject)Resources.Load("BlueGhostPrefab");
    for (int i = 0; i < 4; i++) {
      Vector3 pos = vmap[4,1 + i];
      pghosts[i] = Instantiate(prefab, pos, Quaternion.identity);
      pghosts[i].name = "PlayerBlue";
      omap[4, 1 + i] = pghosts[i];
    }
    prefab = (GameObject)Resources.Load("RedGhostPrefab");
    for (int i = 0; i < 4; i++) {
      Vector3 pos = vmap[5,1 + i];
      pghosts[i + 4] = Instantiate(prefab, pos, Quaternion.identity);
      pghosts[i + 4].name = "PlayerRed";
      omap[5, 1 + i] = pghosts[i + 4];
    }

    // Initialize Enemy Ghosts
    prefab = (GameObject)Resources.Load("BlueGhostPrefab");
    for (int i = 0; i < 4; i++) {
      Vector3 pos = vmap[1,1 + i];
      eghosts[i] = Instantiate(prefab, pos, Quaternion.Euler(0, 0, 180));
      eghosts[i].name = "EnemyBlue";
      omap[1, 1 + i] = eghosts[i];
      var ghost = eghosts[i].GetComponent<GhostCommonControl>();
      ghost.Shadow();
    }
    prefab = (GameObject)Resources.Load("RedGhostPrefab");
    for (int i = 0; i < 4; i++) {
      Vector3 pos = vmap[0,1 + i];
      eghosts[i + 4] = Instantiate(prefab, pos, Quaternion.Euler(0, 0, 180));
      eghosts[i + 4].name = "EnemyRed";
      omap[0, 1 + i] = eghosts[i + 4];
      var ghost = eghosts[i + 4].GetComponent<GhostCommonControl>();
      ghost.Shadow();
    }

  }
  
  // Update is called once per frame
  void Update () {
    
  }

  public void SwapPiece (ref Index2 i1, ref Index2 i2) {
    var obj = omap[i1.r, i1.c];
    omap[i1.r, i1.c] = omap[i2.r, i2.c];
    omap[i2.r, i2.c] = obj;

    omap[i1.r, i1.c].GetComponent<GhostCommonControl>().SetPosition(Index2Pos(i1));
    omap[i2.r, i2.c].GetComponent<GhostCommonControl>().SetPosition(Index2Pos(i2));
  }

  public void MovePiece (ref Index2 from, ref Index2 to) {
    // TODO: null check
    //var obj = omap[to.r, to.c];
    omap[to.r, to.c] = omap[from.r, from.c];
    omap[from.r, from.c] = null;

    omap[to.r, to.c].GetComponent<GhostCommonControl>().SetPosition(Index2Pos(to));
  }

  private int PlayerRedCount = 0;
  private int PlayerBlueCount = 0;

  public void TakePiece (ref Index2 from, ref Index2 to) {
    // TODO: null check
    var obj = omap[to.r, to.c].GetComponent<GhostCommonControl>();
    omap[to.r, to.c] = omap[from.r, from.c];
    omap[from.r, from.c] = null;

    omap[to.r, to.c].GetComponent<GhostCommonControl>().SetPosition(Index2Pos(to));
    obj.SetPosition(Index2Pos(6, p_hunted_.Count));
    obj.Waiting();
    obj.Hide();
    p_hunted_.Add(obj);
    Debug.LogFormat("================: {0}", obj.name);
    if (obj.name == "EnemyRed") {
      var tobj = GameObject.Find ("PlayerRedCount");
      ++PlayerRedCount;
      tobj.GetComponent<Text>().text = String.Format(": {0}", PlayerRedCount);
    } else if (obj.name == "EnemyBlue") {
      var tobj = GameObject.Find ("PlayerBlueCount");
      ++PlayerBlueCount;
      tobj.GetComponent<Text>().text = String.Format(": {0}", PlayerBlueCount);
    }
  }

  private bool IsOutOfInitialArea(ref Index2 idx) {
    if (idx.r < 4 || idx.r > 5 || idx.c < 1 || idx.c > 4) {
      return true;
    }
    return false;
  }

  private bool IsOutOfBattleArea(ref Index2 idx) {
    if (idx.r < 0 || idx.r > 5 || idx.c < 0 || idx.c > 5) {
      return true;
    }
    return false;
  }

  private int SquareDist(ref Index2 from, ref Index2 to) {
    return (to.r - from.r) * (to.r - from.r) + (to.c - from.c) * (to.c - from.c);
  }

  private bool IsSameSide (ref Index2 i1, ref Index2 i2) {
    //ref var name1 = omap[i1.r, i1.c].name;
    //ref var name2 = omap[i2.r, i2.c].name;
    if (IsPlayer(ref i1) && IsPlayer(ref i2)) {
      return true;
    }
    if (IsEnemy(ref i1) && IsEnemy(ref i2)) {
      return true;
    }
    return false;
  }

  private bool IsExists (ref Index2 idx) {
    if (omap[idx.r, idx.c]) {
      return true;
    }
    return false;
  }

  private bool IsPlayer (ref Index2 idx) {
    var name = omap[idx.r, idx.c].name;
    if (name == "PlayerRed" || name == "PlayerBlue") {
      return true;
    }
    return false;
  }

  private bool IsEnemy (ref Index2 idx) {
    var name = omap[idx.r, idx.c].name;
    if (name == "EnemyRed" || name == "EnemyBlue") {
      return true;
    }
    return false;
  }

  private bool IsOutOfRange(ref Index2 from, ref Index2 to) {
    if (IsOutOfBattleArea(ref to)) {
      return true;
    }
    if (SquareDist(ref from, ref to) != 1) {
      return true;
    }
    if (IsExists(ref to) && IsSameSide(ref from, ref to)) {
      return true;
    }
    return false;
  }

  public void Touch () {
    act();
  }

  public void SetupOperation () {
    Vector3 pos = Input.mousePosition;
    Vector3 lpos = Camera.main.ScreenToWorldPoint(pos);
    var idx = Pos2Index(ref lpos);

    if(IsOutOfInitialArea(ref idx)) {
      if (g_obj_) {
        g_obj_.GetComponent<GhostCommonControl>().Waiting();
        g_obj_ = null;
      }
      return;
    }

    var selected = omap[idx.r, idx.c];
    if (g_obj_) {
      if (selected && selected != g_obj_) {
        SwapPiece(ref p_idx_, ref idx);
      } else {
        MovePiece(ref p_idx_, ref idx);
      }
      g_obj_.GetComponent<GhostCommonControl>().Waiting();
      g_obj_ = null;
      //p_idx_   = null;
    } else {
      if (selected) {
        p_idx_ = idx;
        g_obj_ = omap[idx.r, idx.c];
        g_obj_.GetComponent<GhostCommonControl>().Selecting();
      } else {
        Debug.LogFormat("null selected : {0}, {1}", idx.r, idx.c);
      }
    }
  }

  public void PlayerOperation () {
    Debug.LogFormat("PlayerOperation");
    Vector3 pos = Input.mousePosition;
    Vector3 lpos = Camera.main.ScreenToWorldPoint(pos);
    var idx = Pos2Index(ref lpos);

    var selected = omap[idx.r, idx.c];
    if (g_obj_) {
      if(IsOutOfRange(ref p_idx_, ref idx)) {
        Debug.LogFormat(" Out of range : {0}", p_idx_);
        if (g_obj_) {
          g_obj_.GetComponent<GhostCommonControl>().Waiting();
          g_obj_ = null;
        }
        return;
      }

      if (selected && selected != g_obj_) {
        TakePiece(ref p_idx_, ref idx);
      } else {
        MovePiece(ref p_idx_, ref idx);
      }
      g_obj_.GetComponent<GhostCommonControl>().Waiting();
      g_obj_ = null;
      //p_idx_   = null;
    } else {
      if (selected) {
        p_idx_ = idx;
        g_obj_ = omap[idx.r, idx.c];
        g_obj_.GetComponent<GhostCommonControl>().Selecting();
      } else {
        Debug.LogFormat("null selected : {0}, {1}", idx.r, idx.c);
      }
    }
  }
}
