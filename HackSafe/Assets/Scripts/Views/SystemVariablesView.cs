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

    static int CPUUsage = 20;
    static int RAMUsage = 10;
    static int StorageUsage = 1;

    private float update;

    private List<string> progarmmes = new List<string> ();
    private List<int[]> programmesUsage = new List<int[]> ();

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

            List<int[]> tempProgrammeUsage = programmesUsage.GetRange (0, programmesUsage.Count);
            foreach (int[] programmeUsage in tempProgrammeUsage)
            {
                CPU += programmeUsage[0];
                RAM += programmeUsage[1];
                Storage += programmeUsage[2];
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

    public void AddProgramme (string name, int CPU, int RAM, int Storage)
    {
        progarmmes.Add (name);
        programmesUsage.Add (new int[] { CPU, RAM, Storage });
    }
}
