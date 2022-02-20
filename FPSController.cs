using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float x, z;
    public float speed = 0.1f;

    //カメラ取得
    public GameObject cam;
    Quaternion cameraRot, characterRot;
    public float xSens = 3f, ySens = 3f;

    //マウスカーソル表示切り替え
    private bool cursorLock = true;

    //マウス角度制限
    private float minX = -90f, maxX = 90f;

    //アニメーション
    public Animator animator;
        

    
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
    }

    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * ySens;
        float yRot = Input.GetAxis("Mouse Y") * xSens;

        // x, y, z座標を”軸”として回転する
        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);
        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        //キャラの視点変更
        transform.localRotation = characterRot;

        UpdateCursorLock();

        // Fire
        if(Input.GetMouseButton(0))
        {
            animator.SetTrigger("Fire");
        }

        // Reload
        if (Input.GetKey(KeyCode.R))
        {
            animator.SetTrigger("Reload");
        }
        // Walk
        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0) 
        {
            if (!animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", true);
            }
            else if (animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", false);
            }
        }
        // Run
        if (z > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (!animator.GetBool("Run"))
            {
                animator.SetBool("Run", true);
                speed = 0.25f;
            }
        }
        else if (animator.GetBool("Run"))
        {
            animator.SetBool("Run", false);
            speed = 0.1f;
        }
    }

    // 0.02秒ごと呼び出される
    private void FixedUpdate()
    {
        x = 0;
        z = 0;
        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;
        //transform.position += new Vector3(x, 0, z);
        transform.position += cam.transform.forward * z + cam.transform.right * x;
    }
    public void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            cursorLock = false;
        } else if(Input.GetMouseButton(0)) {
            cursorLock = true;
        }

        if (cursorLock) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if (!cursorLock) {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    public Quaternion ClampRotation(Quaternion q) {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
        return q;
    }
}
