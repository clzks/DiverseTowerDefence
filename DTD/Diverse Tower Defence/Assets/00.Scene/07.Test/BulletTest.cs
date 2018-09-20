using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    private UILabel lbTowerType;
    private UILabel lbTowerLevel;
    private UILabel lbTowerUpgrade;

    private UILabel lbTowerAttack;
    private UILabel lbTowerAttackSpeed;
    private UILabel lbTowerAttackType;

    private UILabel lbAttackEnemy;

    private bool isEnemyMode = false;
    private bool isCreateTower = false;
    private bool isAttack = false;
    // ========================= 타워 관련 =========================== //
    public Tower sampleTower;
    public int TowerTypeIndex;
    public int TowerLevel;
    public int CurrUpgrade;
    public GameObject sampleTowerModel;
    // =============================================================== //

    // ========================= 적군 관련 ============================ //
    public GameObject goEnemyAlpha;
    public GameObject goEnemy;
    private int nEnemyId = 0;

    // =============================================================== //

    public Camera MainCamera;
    public Camera UICamera;
    private void Start()
    {
        lbTowerType = GameObject.Find("UI Root/Camera/TowerSettingMode/TypeSetting/Text").GetComponent<UILabel>();
        lbTowerLevel = GameObject.Find("UI Root/Camera/TowerSettingMode/LevelSetting/Text").GetComponent<UILabel>();
        lbTowerUpgrade = GameObject.Find("UI Root/Camera/TowerSettingMode/UpgradeSetting/Text").GetComponent<UILabel>();
        lbTowerAttack = GameObject.Find("UI Root/Camera/TowerSettingMode/TowerInfo/Attack").GetComponent<UILabel>();
        lbTowerAttackSpeed = GameObject.Find("UI Root/Camera/TowerSettingMode/TowerInfo/AttackSpeed").GetComponent<UILabel>();
        lbTowerAttackType = GameObject.Find("UI Root/Camera/TowerSettingMode/TowerInfo/AttackType").GetComponent<UILabel>();
        lbAttackEnemy = GameObject.Find("UI Root/Camera/ConstructMode/Attack_Stop/Label").GetComponent<UILabel>();
        //goEnemyAlpha = ;
        goEnemyAlpha = Instantiate(Resources.Load<GameObject>("EnemySampleAlpha"));
        //goEnemy = Instantiate(EnemyManager.Instance.EnemyModelList[0]);

        MainCamera = Camera.main;
        UICamera = GameObject.Find("UI Root/Camera").GetComponent<Camera>();
        //EnemyManager.Instance.MakeEnemyDamagePool();
        EnemyManager.Instance.BarParent = GameObject.Find("UI Root/Camera/ProgressBar").transform;
        BulletManager.Instance.MakeBulletPool();

    }

    private void Update()
    {
        if(sampleTower != null)
        {
            lbTowerType.text = sampleTower.Name;
            lbTowerLevel.text = TowerLevel.ToString();
            lbTowerUpgrade.text = CurrUpgrade.ToString();

            if (sampleTower.BulletType != 2)
                lbTowerAttack.text = (sampleTower.Attack + sampleTower.UpgradePerDamage * CurrUpgrade).ToString();

            lbTowerAttackSpeed.text = sampleTower.AtkSpd.ToString();
            
            switch (sampleTower.BulletType)
            {
                case 0:
                    lbTowerAttackType.text = "즉발";
                    break;

                case 1:
                    lbTowerAttackType.text = "투사체";
                    break;

                case 2:
                    lbTowerAttackType.text = "광선";
                    break;

                case 3:
                    lbTowerAttackType.text = "멀티샷";
                    break;

                case 4:
                    lbTowerAttackType.text = "스플래쉬";
                    break;

                case 5:
                    lbTowerAttackType.text = "바운싱";
                    break;
            }
        }

        if(isEnemyMode)
        {
            goEnemyAlpha.SetActive(true);
            CreateEnemy();
        }
        else
        {
            goEnemyAlpha.SetActive(false);
        }

        if(isAttack)
        {
            lbAttackEnemy.text = "공격";
            if (sampleTower != null)
                sampleTower.isStart = true;
        }
        else
        {
            lbAttackEnemy.text = "공격 중지";
            if (sampleTower != null)
                sampleTower.isStart = false;
        }
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    public void CreateTower()
    {
        if (!isCreateTower)
        {
            Vector3 v = GameObject.Find("Constructable Place/0").transform.position;
            sampleTowerModel = Instantiate(ConstructManager.Instance.towerModelList[0]);
            sampleTowerModel.transform.position = v;
            sampleTowerModel.AddComponent<Tower>();
            sampleTower = sampleTowerModel.GetComponent<Tower>();
            TowerTypeIndex = 0;
            TowerLevel = 0;
            CurrUpgrade = 0;
            ConstructManager.Instance.UserTowerDic.Add(0, sampleTowerModel);
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic["Range0"][TowerLevel]);
            sampleTower.isStart = false;
            isCreateTower = true;
        }
    }

    public void ChangeTypeLeft()
    {
        if(TowerTypeIndex == 0)
        {
            TowerTypeIndex = 33;
            string tempKey = ConstructManager.Instance.towerTypeList[TowerTypeIndex].TowerName;
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic[tempKey][TowerLevel]);
        }
        else
        {
            TowerTypeIndex -= 1;
            string tempKey = ConstructManager.Instance.towerTypeList[TowerTypeIndex].TowerName;
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic[tempKey][TowerLevel]);
        }
    }

    public void ChangeTypeRight()
    {
        if (TowerTypeIndex == 33)
        {
            TowerTypeIndex = 0;
            string tempKey = ConstructManager.Instance.towerTypeList[TowerTypeIndex].TowerName;
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic[tempKey][TowerLevel]);
        }
        else
        {
            TowerTypeIndex += 1;
            string tempKey = ConstructManager.Instance.towerTypeList[TowerTypeIndex].TowerName;
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic[tempKey][TowerLevel]);
        }
    }

    public void ChangeLevelPlus()
    {
        if (TowerLevel < 4)
        {
            TowerLevel += 1;
            string tempKey = ConstructManager.Instance.towerTypeList[TowerTypeIndex].TowerName;
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic[tempKey][TowerLevel]);
        }
    }
    public void ChangeLevelMinus()
    {
        if (TowerLevel > 0)
        {
            TowerLevel -= 1;
            string tempKey = ConstructManager.Instance.towerTypeList[TowerTypeIndex].TowerName;
            sampleTower.SetTowerStatus(ConstructManager.Instance.towerStatusDic[tempKey][TowerLevel]);
        }
    }
    public void ChangeUpgradePlus()
    {
        if (CurrUpgrade < 255)
        {
            CurrUpgrade += 1;
            sampleTower.CurrUpgradeNum += 1;
        }
    }
    public void ChangeUpgradeMinus()
    {
        if (CurrUpgrade >= 1)
        {
            CurrUpgrade -= 1;
            sampleTower.CurrUpgradeNum -= 1;
        }
    }
    public void MoveCamera()
    {
        if(Input.GetKey(KeyCode.A))
        {
            MainCamera.transform.position += Vector3.left * 0.5f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MainCamera.transform.position += Vector3.right * 0.5f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            MainCamera.transform.position += Vector3.forward * 0.5f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MainCamera.transform.position += Vector3.back * 0.5f;
        }
    }
    public void OnClickCreateEnemy()
    {
        if (isEnemyMode)
            isEnemyMode = false;
        else
            isEnemyMode = true;
    }

    public void OnClickAttackEnemy()
    {
        if (isAttack)
            isAttack = false;
        else
            isAttack = true;
    }

    public void CreateEnemy()
    {
        bool isUIhit = false;
        RaycastHit rayHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin, ray.direction, out rayHit, float.MaxValue, 1 << 12))
        {
            goEnemyAlpha.transform.position = rayHit.point + new Vector3(0, 0.5f, 0);
        }
        
        ray = UICamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin, ray.direction, out rayHit, float.MaxValue, 1 << 8))
        {
            isUIhit = true;
        }

        if(isEnemyMode)
        {
            if (Input.GetMouseButtonDown(0) && !isUIhit)
            {
                goEnemy = Instantiate(EnemyManager.Instance.EnemyModelList[1]);
                goEnemy.AddComponent<Enemy>();
                Enemy e = goEnemy.GetComponent<Enemy>();
                e.SetEnemy(EnemyManager.Instance.EnemyStatusList[0], nEnemyId, new List<int>());
                goEnemy.transform.position = goEnemyAlpha.transform.position;
                goEnemy.name = "Enemy" + nEnemyId.ToString();
                nEnemyId++;
            }
        }
    }

    
}
