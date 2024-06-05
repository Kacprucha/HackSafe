using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadigTerminal : MonoBehaviour
{
    [SerializeField] GameObject terminalResponsePrefab;

    int stage = 0;
    float waitingTime = 0f;

    List<string> baner = new List<string> () { " _    _            _     _____        __  _       ", "| |  | |          | |   / ____|      / _|| |      ", "| |__| | __ _  ___| | _| (___  __ _ | |_ | |_   _ ",
                                                @"|  __  |/ _` |/ __| |/ /\___ \/ _` ||  _|| | | | |", "| |  | | (_| | (__|   < ____) |(_| || |  | | |_| |", @"|_|  |_|\__,_|\___|_|\_\_____/\__,_||_|  |_|\__, |",
                                                "                                             __/ |", "                                            |___/ "};

    List<string> start = new List<string> () { "HackSafe System Booting Up...", "--------------------------------" };

    List<string> inicialize = new List<string> () { "[INFO] Initializing HackSafe Security Modules...", "[INFO] Loading configuration files from /etc/hacksafe/config.yaml", "[INFO] Configuration loaded successfully.",
                                                    "[INFO] Starting core services...", " "};

    List<string> butap = new List<string> () { "[ OK ] Network Service initialized.", "[ OK ] Database Connection established.", "[ OK ] Firewall Service activated.", "[ OK ] Intrusion Detection System (IDS) started.",
                                                "[ OK ] Log Monitoring Service initiated.", "[ OK ] Anti-Malware Engine started.", " "};

    List<string> diagnostic = new List<string> () { "[INFO] Running system diagnostics..." , "[ OK ] CPU check: All cores operational.", "[ OK ] Memory check: 8192MB available, 2048MB used.", "[ OK ] Disk check: 256GB available, 74GB used." , 
                                                    "[ OK ] Network check: Connection to primary server established.", "[ OK ] Power Supply check: Stable.", " " };

    List<string> seciurity = new List<string> () { "[INFO] Verifying security protocols...", "[ OK ] Authentication service is online.", "[ OK ] Encryption modules are active.", "[ OK ] Security updates are current.", 
                                                    "[ OK ] Rootkit scanner completed: No threats detected.", "[ OK ] Vulnerability assessment: No critical issues found.", " " };

    List<string> auxiliary = new List<string> () { "[INFO] Starting auxiliary services...", "[ OK ] Email Notification Service is active.", "[ OK ] Backup Service scheduled at 02:00 AM.", "[ OK ] Audit Logging Service initiated.",
                                                    "[ OK ] Performance Monitoring Service is running.", "[ OK ] System Update Service is ready.", " "};

    List<string> checks = new List<string> () { "[INFO] Running environment checks...", "[ OK ] Temperature check: Optimal.", "[ OK ] Humidity check: Within safe range.", "[ OK ] Noise level check: Normal.", " " };

    List<string> network = new List<string> () { "[INFO] Performing network tests...", "[ OK ] Latency test: 20ms.", "[ OK ] Bandwidth test: 100Mbps.", "[ OK ] Packet loss test: 0%.", "[ OK ] DNS resolution: Successful.", " " };

    List<string> interface1 = new List<string> () { "[INFO] Launching HackSafe user interface...", "[INFO] Starting GUI services...", "[OK] GUI services initialized." };
    List<string> interface2 = new List<string> () { "[ OK ] User session manager started.", "[ OK ] Dashboard is loading..." };

    // Start is called before the first frame update
    void Start()
    {
        foreach(string line in baner)
        {
            GameObject baner = Instantiate (terminalResponsePrefab);
            baner.GetComponent<PassiveTerminalElement> ().UpdateText (line);
            baner.GetComponent<PassiveTerminalElement> ().SetTextToBestFit ();
            baner.transform.SetParent (this.gameObject.transform);
            baner.transform.localScale = new Vector3 (1, 1, 1);
        }

        foreach(string line in start)
        {
            GameObject tr = Instantiate (terminalResponsePrefab);
            tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
            tr.transform.SetParent (this.gameObject.transform);
            tr.transform.localScale = new Vector3 (1, 1, 1);
        }

        StartDelay (0.5f);
    }

    // Update is called once per frame
    void Update ()
    {
        float randomNumber = Random.Range (0.5f, 2f);
        waitingTime += randomNumber;
        StartDelay (waitingTime);
    }

    public void StartDelay (float delayInSeconds)
    {
        StartCoroutine (DelayCoroutine (delayInSeconds));
    }

    private IEnumerator DelayCoroutine (float delayInSeconds)
    {
        yield return new WaitForSeconds (delayInSeconds);

        switch (stage)
        {
            case 0:

                foreach (string line in inicialize)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

            case 1:

                foreach (string line in butap)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

            case 2:

                foreach (string line in diagnostic)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

            case 3:

                foreach (string line in seciurity)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

            case 4:

                foreach (string line in auxiliary)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

            case 5:

                foreach (string line in checks)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

            case 6:

                foreach (string line in network)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                StartCoroutine (PrepareScene ());

                stage++;

                break;

            case 7:

                foreach (string line in interface1)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                foreach (string line in interface2)
                {
                    GameObject tr = Instantiate (terminalResponsePrefab);
                    tr.GetComponent<PassiveTerminalElement> ().UpdateText (line);
                    tr.transform.SetParent (this.gameObject.transform);
                    tr.transform.localScale = new Vector3 (1, 1, 1);
                }

                stage++;

                break;

        }
    }

    IEnumerator PrepareScene ()
    {
        yield return new WaitForSeconds (3f);
        SceneManager.LoadScene ("Gameplay");
    }
}
