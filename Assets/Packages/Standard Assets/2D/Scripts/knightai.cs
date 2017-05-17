using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knightai : MonoBehaviour {

    private bool m_FacingRight = true;
    bool patrol;
    
    public int pastchoice = 1;
    public int walkway;
    bool shouldflip1;

    public int timer;
    public bool timerpure = false;
    public int timer2;
    public int x = 0;
    public Vector3 theScale;
    int timer3;
    public int idea1;
    public int walktimer = 20;
    int walkspeed = 6;
    public Transform knight;
    Rigidbody2D m_Rigidbody2D;
    private Animator m_Anim;

    void Start () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        theScale = transform.localScale;
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

    }

    void Update() {
        m_Anim.SetFloat("Speed", m_Rigidbody2D.velocity.x);

        if (patrol == true)
        {
            walkspeed = 3;
        }
        int idea = Random.Range(0, 2);
        if (idea == 1)
        {
            
            patrol = true;

            timer++;
            if (timer3 < 51)
            timer3++;

            if (timer == 48 && idea > 0)
            {
                idea1 = Random.Range(0, 2);
                walkway = Random.Range(1, 3);
                timer = 0;
            }
            if (idea1 == 1)
            {

                timer2++;

                if (timer3 == 49)
                {
                    

                    walktimer = Random.Range(10, 60);
                    timerpure = false;

                }
                if (timer2 < timer3 + walktimer && timerpure == false)
                {
                    x++;
                    if (walkway == 1)
                        if (pastchoice == -1)
                        {
                            if (theScale.x == -1.77f)
                            {
                                theScale.x *= -1;
                                transform.localScale = theScale;
                            }

                        }
                    pastchoice = 1;
                        m_Rigidbody2D.velocity = new Vector2(pastchoice * walkspeed, m_Rigidbody2D.velocity.y);
                    if (walkway == 2)
                    {
                        if (pastchoice == 1)
                        {
                            if (theScale.x == 1.77f)
                            {
                                theScale.x *= -1;
                                transform.localScale = theScale;
                            }
                        }

                        pastchoice = -1;
                       
                        m_Rigidbody2D.velocity = new Vector2(pastchoice * walkspeed, m_Rigidbody2D.velocity.y);

                    }
                    }
                if (timer2 > timer3 + walktimer)
                {
                    
                    timer2 = 0;
                    timer3 = 0;
                    x = 0;

                }
                /* if(timer < 50)
                 m_Rigidbody2D.velocity = new Vector2(1 * walkspeed, m_Rigidbody2D.velocity.y);
                 if(timer > 50)
                     m_Rigidbody2D.velocity = new Vector2(-1 * walkspeed, m_Rigidbody2D.velocity.y);
                 if (timer == 100)
                 {
                     timer = 0;
                     Flip();
                     knight.transform.position = new Vector2(knight.transform.position.x + 1, knight.transform.position.y);
                 }
                 if (timer == 50)
                 {
                     Flip();
                     knight.transform.position = new Vector2(knight.transform.position.x - 1, knight.transform.position.y);
                 }
            */
            }
        }
    }
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
