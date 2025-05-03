package com.example.userapi.model;

import jakarta.persistence.*;
import java.sql.Timestamp;

@Entity
@Table(name = "users")
public class User {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, unique = true)
    private String username;

    @Column(nullable = false)
    private String password;

    @Column(nullable = false)
    private Timestamp registerTime;

    @Column(nullable = false)
    private Integer gold;

    // getters & setters…

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }

    public String getUsername() { return username; }
    public void setUsername(String u) { this.username = u; }

    public String getPassword() { return password; }
    public void setPassword(String p) { this.password = p; }

    public Timestamp getRegisterTime() { return registerTime; }
    public void setRegisterTime(Timestamp t) { this.registerTime = t; }

    public Integer getGold() { return gold; }
    public void setGold(Integer g) { this.gold = g; }
}
