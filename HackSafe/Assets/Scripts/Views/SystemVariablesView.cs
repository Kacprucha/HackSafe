using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemVariablesView : MonoBehaviour
{
    [SerializeField] Slider CPUSlider;
    [SerializeField] Slider RAMSlider;
    [SerializeField] Slider StorageSlider;

    [SerializeField] Text CPULabel;
    [SerializeField] Text RAMLabel;
    [SerializeField] Text StorageLabel;

    [SerializeField] Text CPUProgramList;
    [SerializeField] Text RAMProgramList;
    [SerializeField] Text StorageProgramList;

    static int CPUUsage = 20;
    static int RAMUsage = 10;
    static int StorageUsage = 1;

    private float update;

    private List<string> programs = new List<string> ();
    private List<int[]> programsUsage = new List<int[]> ();

    // Start is called before the first frame update
    void Start()
    {
        update = 1.0f;   
    }

    // Update is called once per frame
    void Update()
    {
        update += Time.deltaTime;
        if (update >= 1.0f)
        {
            update = 0.0f;

            float CPU = CPUUsage;
            float RAM = RAMUsage;
            float Storage = StorageUsage;

            CPUProgramList.text = "";
            RAMProgramList.text = "";
            StorageProgramList.text = "";

            List<int[]> tempProgrammeUsage = programsUsage.GetRange (0, programsUsage.Count);
            for (int i = 0; i < tempProgrammeUsage.Count; i++)
            {
                CPU += tempProgrammeUsage[i][0];
                RAM += tempProgrammeUsage[i][1];
                Storage += tempProgrammeUsage[i][2];

                if (tempProgrammeUsage[i][0] > 0)
                {
                    CPUProgramList.text += programs[i] + ": " + tempProgrammeUsage[i][0].ToString () + "%\n";
                }
               
                if (tempProgrammeUsage[i][1] > 0)
                {
                    RAMProgramList.text += programs[i] + ": " + tempProgrammeUsage[i][1].ToString () + "%\n";
                }

                if (tempProgrammeUsage[i][2] > 0)
                {
                    StorageProgramList.text += programs[i] + ": " + tempProgrammeUsage[i][2].ToString () + "%\n";
                }
            }

            CPU += Random.Range (1, 4);
            RAM += Random.Range (1, 4);
            Storage += Random.Range (1, 4);

            CPU = Mathf.Clamp (CPU, 0, 100);
            RAM = Mathf.Clamp (RAM, 0, 100);
            Storage = Mathf.Clamp (Storage, 0, 100);

            CPULabel.text = CPU.ToString () + "%";
            RAMLabel.text = RAM.ToString () + "%";
            StorageLabel.text = Storage.ToString () + "%";

            CPUSlider.value = CPU / 100;
            RAMSlider.value = RAM / 100;
            StorageSlider.value = Storage / 100;
        }
    }

    public void AddProgram (string name, int CPU, int RAM, int Storage)
    {
        programs.Add (name);
        programsUsage.Add (new int[] { CPU, RAM, Storage });
    }

    public void DelateProgram (string name)
    {
        int position = -1;
        for (int i = 0; i < programs.Count; i++)
        {
            if (programs[i] == name)
            {
                position = i;
                break;
            }
        }

        if (position >= 0)
        {
            programsUsage.RemoveAt (position);
            programs.RemoveAt (position);
        }
    }

    public void UpdateProgramInpact (string name, int CPU, int RAM, int Storage)
    {
        int position = -1;
        for (int i = 0; i < programs.Count; i++)
        {
            if (programs[i] == name)
            {
                position = i;
                break;
            }
        }

        if (position >= 0)
        {
            programsUsage[position][0] = CPU;
            programsUsage[position][1] = RAM;
            programsUsage[position][2] = Storage;
        }
    }
}
